using CustomSabersLite.Models;
using CustomSabersLite.UI;
using CustomSabersLite.Utilities.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Utilities;

internal class SaberMetadataCache(CustomSabersLoader saberLoader, SaberListManager saberListManager, SpriteCache spriteCache, SaberInstanceManager saberInstances, SaberMetadataCacheMigrationManager migrationManager) : IInitializable
{
    private readonly CustomSabersLoader customSabersLoader = saberLoader;
    private readonly SaberListManager saberListManager = saberListManager;
    private readonly SpriteCache spriteCache = spriteCache;
    private readonly SaberInstanceManager saberInstanceManager = saberInstances;
    private readonly SaberMetadataCacheMigrationManager saberMetadataCacheMigrationManager = migrationManager;

    private string MetadataFileName => "metadata.json";
    private string CacheArchiveFileName => "cache";
    private string CacheArchiveFilePath => Path.Combine(PluginDirs.UserData.FullName, CacheArchiveFileName);

    internal record Progress(bool Completed, string Stage, int? StagePercent = null);

    public event Action<Progress>? LoadingProgressChanged;

    private Progress currentProgress = new(false, string.Empty, 0);
    public Progress CurrentProgress
    {
        get => currentProgress;
        private set
        {
            if (currentProgress != value)
            {
                currentProgress = value;
                LoadingProgressChanged?.Invoke(value);
            }
        }
    }

    public async void Initialize() => await ReloadAsync();

    public async Task ReloadAsync()
    {
        saberInstanceManager.Clear(false);
        saberListManager.Clear();
        CurrentProgress = new(false, "Reloading");

        try
        {
            var stopwatch = Stopwatch.StartNew();

            string[]? installedSaberPaths = FileUtils.GetFilePaths(
                PluginDirs.CustomSabers.FullName, [".saber", ".whacker"], SearchOption.AllDirectories, true).ToArray();

            var cacheFile = await InternalReloadAsync(installedSaberPaths);
            stopwatch.Stop();
            Logger.Notice($"Cache loading took {stopwatch.ElapsedMilliseconds}ms");


            var installedSabersMetadata = cacheFile.CachedMetadata.Where(meta => installedSaberPaths.Contains(meta.RelativePath));
            var saberMetadata = installedSabersMetadata.Select(m =>
                new CustomSaberMetadata(
                    new SaberFileInfo(Path.Combine(PluginDirs.CustomSabers.FullName, m.RelativePath), m.Hash, m.DateAdded, m.SaberType),
                    m.LoaderError,
                    new Descriptor(m.SaberName, m.AuthorName, spriteCache.GetSprite(m.RelativePath))));

            #if SHADER_DEBUG
            ShaderInfoDump.Instance.DumpTo(PluginDirs.UserData.FullName);
            #endif

            saberListManager.SetData(saberMetadata);
        }
        catch (Exception ex)
        {
            Logger.Critical($"Problem encountered during cache initialization - the mod will not activate\n{ex}");
        }

        CurrentProgress = new(true, "Completed");
    }

    private async Task<CacheFileModel> InternalReloadAsync(string[] saberRelativePaths)
    {
        if (!await saberMetadataCacheMigrationManager.MigrationTask)
        {
            Logger.Warn("Internal reload was denied because of a failure during cache migration");
            return CacheFileModel.Empty;
        }

        CurrentProgress = currentProgress with { Stage = "Loading Cache" };
        var localCacheFile = await GetLocalCache();
        Logger.Notice($"Found {localCacheFile.CachedMetadata.Length} cached saber entries");

        CurrentProgress = currentProgress with { Stage = "Updating Cache" }; 
        var updatedCacheFile = await GetUpdatedCache(localCacheFile, saberRelativePaths);

        if (localCacheFile != updatedCacheFile)
        {
            CurrentProgress = currentProgress with { Stage = "Saving Metadata" };
            await SaveMetadataToCache(updatedCacheFile);
        }

        return updatedCacheFile;
    }

    private async Task<CacheFileModel> GetLocalCache()
    {
        if (!File.Exists(CacheArchiveFilePath)) return CacheFileModel.Empty;

        using var cacheZipArchive = ZipFile.OpenRead(CacheArchiveFilePath);

        var metadataJsonEntry = cacheZipArchive.GetEntry(MetadataFileName);
        using var metadataJsonStream = metadataJsonEntry?.Open();

        if (metadataJsonStream is null) return CacheFileModel.Empty;

        var cacheFile = JsonReading.DeserializeStream<CacheFileModel>(metadataJsonStream) ?? CacheFileModel.Empty;

        foreach (var meta in cacheFile.CachedMetadata)
        {
            var sprite = await LoadSpriteFromEntry(cacheZipArchive.GetEntry($"images/{meta.Hash}.png"));
            spriteCache.AddSprite(meta.RelativePath, sprite);
        }

        return cacheFile;
    }

    private async Task SaveMetadataToCache(CacheFileModel cacheFile)
    {
        var tempCacheDir = Directory.CreateDirectory(Path.Combine(PluginDirs.UserData.FullName, "temp"));
        var imagesDir = tempCacheDir.CreateSubdirectory("images");

        try
        {
            foreach (var meta in cacheFile.CachedMetadata)
            {
                byte[]? imageData = spriteCache.GetSprite(meta.RelativePath)?.texture.EncodeToPNG();
                if (imageData == null)
                    continue;

                string? imagePath = Path.Combine(imagesDir.FullName, meta.Hash + ".png");
                await File.WriteAllBytesAsync(imagePath, imageData);
            }

            string? cacheJson = JsonConvert.SerializeObject(cacheFile, Formatting.None);
            string? metadataFilePath = Path.Combine(tempCacheDir.FullName, MetadataFileName);
            await File.WriteAllTextAsync(metadataFilePath, cacheJson);

            if (File.Exists(CacheArchiveFilePath)) File.Delete(CacheArchiveFilePath);
            ZipFile.CreateFromDirectory(tempCacheDir.FullName, CacheArchiveFilePath);
        }
        finally
        {
            tempCacheDir.Delete(true);
        }
    }

    private async Task<CacheFileModel> GetUpdatedCache(CacheFileModel existingCache, string[] installedSabers)
    {
        /*var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Math.Max(1, Environment.ProcessorCount / 2 - 1),
            CancellationToken = CancellationToken.None
        };

        var installedSaberHashes = new Dictionary<string, string>();
        await Task.Run(() => Parallel.ForEach(installedSabers, parallelOptions, (path) =>
        {
            var hash = Hashing.MD5Checksum(Path.Combine(PluginDirs.CustomSabers.FullName, path), "x2");
            if (hash != null) installedSaberHashes.TryAdd(path, hash);
        }));*/

        var loadedMetadata = new List<SaberMetadataModel>();

        // file paths should always be distinct and are used as a key
        var cachedSaberPaths = existingCache.CachedMetadata.Select(m => m.RelativePath).ToHashSet();

        var notCachedSabers = installedSabers.WhereNot(cachedSaberPaths.Contains);

        if (notCachedSabers.Any())
        {
            loadedMetadata.AddRange(await LoadMetadataFromSabers(notCachedSabers));
        }

        // todo - determine if a file's hash has changed and load it

        // no new sabers were found, so continue with existing cache, otherwise update it
        return !loadedMetadata.Any() ? existingCache
            : new CacheFileModel(Plugin.Version.ToString(), [..loadedMetadata, ..existingCache.CachedMetadata]);
    }

    private async Task<List<SaberMetadataModel>> LoadMetadataFromSabers(IEnumerable<string> sabersForCaching)
    {
        CurrentProgress = currentProgress with { Stage = "Loading Sabers" };

        string[]? relativePaths = sabersForCaching.ToArray();
        var loadedSaberMetadata = new List<SaberMetadataModel>();
        int currentItem = 1;

        foreach (string? relativePath in relativePaths)
        {
            using var saberData = await customSabersLoader.GetSaberData(relativePath, false);
            var metadata = saberData.Metadata;

            if (metadata.SaberFile.Hash != null)
            {
                loadedSaberMetadata.Add(new SaberMetadataModel(relativePath, metadata.SaberFile.Hash, metadata.SaberFile.Type, metadata.LoaderError, metadata.Descriptor.SaberName.FullName, metadata.Descriptor.AuthorName.FullName, metadata.SaberFile.DateAdded));
            }

            CurrentProgress = currentProgress with { StagePercent = currentItem * 100 / relativePaths.Length };
            currentItem++;
        }

        CurrentProgress = currentProgress with { StagePercent = null };
        return loadedSaberMetadata;
    }

    private static async Task<Sprite?> LoadSpriteFromEntry(ZipArchiveEntry? entry)
    {
        if (entry is null) return null;
        using var ms = new MemoryStream();
        using var s = entry.Open();
        await s.CopyToAsync(ms);
        return new Texture2D(2, 2).ToSprite(ms.ToArray());
    }
}
