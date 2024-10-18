using CustomSabersLite.Components.Managers;
using CustomSabersLite.Models;
using CustomSabersLite.UI.Managers;
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

internal class SaberMetadataCache(CustomSabersLoader saberLoader, SaberListManager saberListManager, SpriteCache spriteCache, SaberInstanceManager saberInstances, SaberMetadataCacheMigrationManager migrationManager, ITimeService timeService) : IInitializable
{
    private readonly CustomSabersLoader customSabersLoader = saberLoader;
    private readonly SaberListManager saberListManager = saberListManager;
    private readonly SpriteCache spriteCache = spriteCache;
    private readonly SaberInstanceManager saberInstanceManager = saberInstances;
    private readonly SaberMetadataCacheMigrationManager saberMetadataCacheMigrationManager = migrationManager;
    private readonly ITimeService timeService = timeService;

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
            currentProgress = value;
            LoadingProgressChanged?.Invoke(value);
        }
    }

    public async void Initialize() => await ReloadAsync();

    public async Task ReloadAsync()
    {
        CurrentProgress = new(false, "Reloading");

        try
        {
            var stopwatch = Stopwatch.StartNew();
            await InternalReloadAsync();
            stopwatch.Stop();
            Logger.Info($"Cache loading took {stopwatch.ElapsedMilliseconds}ms");
        }
        catch (Exception ex)
        {
            Logger.Critical($"Problem encountered during cache initialization - the mod will not activate\n{ex}");
        }

        CurrentProgress = new(true, "Completed");
    }

    private async Task InternalReloadAsync()
    {
        saberInstanceManager.Clear(false);
        saberListManager.SetData([]);

        if (!await saberMetadataCacheMigrationManager.MigrationTask)
        {
            Logger.Warn("Internal reload was denied because of a failure during cache migration");
            return;
        }

        var installedSaberPaths = FileUtils.GetFilePaths(
            PluginDirs.CustomSabers.FullName, [FileExts.Saber, FileExts.Whacker], SearchOption.AllDirectories, true).ToArray();

        var localCacheFile = await GetLocalCache();
        Logger.Notice($"Found {localCacheFile.CachedMetadata.Length} cached saber entries");

        var updatedCacheFile = await GetUpdatedCache(localCacheFile, installedSaberPaths);

        if (localCacheFile != updatedCacheFile)
            await SaveMetadataToCache(updatedCacheFile);

        var installedSabersMetadata = updatedCacheFile.CachedMetadata.Where(meta => installedSaberPaths.Contains(meta.RelativePath));
        var saberMetadata = installedSabersMetadata.Select(m =>
            new CustomSaberMetadata(
                new SaberFileInfo(m.RelativePath, m.Hash, m.SaberType),
                m.LoaderError,
                new Descriptor(m.SaberName, m.AuthorName, spriteCache.GetSprite(m.RelativePath))));

        saberListManager.SetData(saberMetadata);
    }

    private async Task<CacheFileModel> GetLocalCache()
    {
        CurrentProgress = currentProgress with { Stage = "Loading Cache" };

        if (!File.Exists(CacheArchiveFilePath))
            return CacheFileModel.Empty;

        using var cacheZipArchive = ZipFile.OpenRead(CacheArchiveFilePath);

        var metadataJsonEntry = cacheZipArchive.GetEntry(MetadataFileName);
        using var metadataJsonStream = metadataJsonEntry?.Open();

        if (metadataJsonStream is null)
            return CacheFileModel.Empty;

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
        CurrentProgress = currentProgress with { Stage = "Saving Metadata" };

        var tempCacheDir = Directory.CreateDirectory(Path.Combine(PluginDirs.UserData.FullName, "temp"));
        var imagesDir = tempCacheDir.CreateSubdirectory("images");

        try
        {
            foreach (var meta in cacheFile.CachedMetadata)
            {
                var imageData = spriteCache.GetSprite(meta.RelativePath)?.texture.EncodeToPNG();
                if (imageData == null)
                    continue;

                var imagePath = Path.Combine(imagesDir.FullName, meta.Hash + ".png");
                await File.WriteAllBytesAsync(imagePath, imageData);
            }

            var cacheJson = JsonConvert.SerializeObject(cacheFile, Formatting.None);
            var metadataFilePath = Path.Combine(tempCacheDir.FullName, MetadataFileName);
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
        CurrentProgress = currentProgress with { Stage = "Updating Cache" };

        var installedSaberHashes = installedSabers.Select(s => 
            (Path: s, Hash: Hashing.MD5Checksum(Path.Combine(PluginDirs.CustomSabers.FullName, s), "x2")))
            .ToArray();

        // todo - update cache if a file is re-added into the folder with a new date
        /*
        var installedMetadata = existingCache.CachedMetadata
            .Where(m => installedSaberHashes.Any(x => m.Hash == x.Hash))
            .ToArray();
        */

        var cachedMetaHashes = existingCache.CachedMetadata.ToDictionary(m => m.RelativePath);
        var sabersToLoadForCaching = installedSabers.WhereNot(cachedMetaHashes.ContainsKey);
        if (!sabersToLoadForCaching.Any())
        {
            // no new sabers were found, so continue with existing cache
            return existingCache;
        }

        var loadedMetadata = await LoadMetadataFromSabers(sabersToLoadForCaching);
        loadedMetadata.ForEach(m => cachedMetaHashes.Add(m.RelativePath, m));

        return new CacheFileModel(Plugin.Version.ToString(), cachedMetaHashes.Values.ToArray());
    }

    private async Task<List<SaberMetadataModel>> LoadMetadataFromSabers(IEnumerable<string> sabersForCaching)
    {
        CurrentProgress = currentProgress with { Stage = "Loading Sabers" };

        var relativePaths = sabersForCaching.ToArray();
        var saberMetadata = new List<SaberMetadataModel>();
        var itemsCount = relativePaths.Length;
        var currentItem = 0;
        var lastPercent = 0;
        var currentTime = timeService.GetUtcTime();

        foreach (var saber in relativePaths)
        {
            using var saberData = await customSabersLoader.GetSaberData(saber, false);

            saberMetadata.Add(new SaberMetadataModel(
                saberData.Metadata.FileInfo.RelativePath ?? throw new ArgumentNullException(),
                saberData.Metadata.FileInfo.Hash ?? throw new ArgumentNullException(),
                saberData.Metadata.FileInfo.Type,
                saberData.Metadata.LoaderError,
                saberData.Metadata.Descriptor.SaberName.FullName,
                saberData.Metadata.Descriptor.AuthorName.FullName,
                currentTime));

            var currentPercent = (currentItem + 1) * 100 / itemsCount;
            if (currentPercent > lastPercent)
            {
                CurrentProgress = currentProgress with { StagePercent = currentPercent };
                lastPercent = currentPercent;
            }
            currentItem++;
        }

        CurrentProgress = currentProgress with { StagePercent = null };
        return saberMetadata;
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
