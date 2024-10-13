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

internal class SaberMetadataCache(CustomSabersLoader saberLoader, SaberListManager saberListManager, SpriteCache spriteCache, SaberInstanceManager saberInstances) : IInitializable
{
    private readonly CustomSabersLoader customSabersLoader = saberLoader;
    private readonly SaberListManager saberListManager = saberListManager;
    private readonly SpriteCache spriteCache = spriteCache;
    private readonly SaberInstanceManager saberInstanceManager = saberInstances;

    private string MetadataFileName => "metadata.json";
    private string CacheArchiveFileName => "cache";
    private string CacheArchiveFilePath => Path.Combine(PluginDirs.UserData.FullName, CacheArchiveFileName);

    public event Action<int>? LoadingProgressChanged;
    public event Action? LoadingComplete;

    public bool InitializationFinished { get; private set; }

    public async void Initialize() =>
        await ReloadAsync();

    public async Task ReloadAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await InternalReloadAsync();
        }
        catch (Exception ex)
        {
            Logger.Critical($"Problem encountered during cache initialization - the mod will not activate\n{ex}");
        }

        InitializationFinished = true;
        LoadingComplete?.Invoke();

        stopwatch.Stop();
        Logger.Info($"Cache loading took {stopwatch.ElapsedMilliseconds}ms");
    }

    private async Task InternalReloadAsync()
    {
        saberInstanceManager.Clear(false);
        saberListManager.SetData([]);
        LoadingProgressChanged?.Invoke(0);

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
                new Descriptor(m.SaberName, m.AuthorName, spriteCache.GetSprite(m.RelativePath)),
                new SaberModelFlags(m.IncompatibleShaders, m.IncompatibleShaderNames)));

        saberListManager.SetData(saberMetadata);
    }

    private async Task<CacheFileModel> GetLocalCache()
    {
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
        Logger.Info("Saving cache");

        var tempCacheDir = Directory.CreateDirectory(Path.Combine(PluginDirs.UserData.FullName, "temp"));
        var imagesDir = tempCacheDir.CreateSubdirectory("images");

        foreach (var meta in cacheFile.CachedMetadata)
        {
            var imageData = spriteCache.GetSprite(meta.RelativePath)?.texture.EncodeToPNG();
            if (imageData == null)
                continue;

            var imagePath = Path.Combine(imagesDir.FullName, meta.Hash + ".png");
            if (!File.Exists(imagePath))
                await File.WriteAllBytesAsync(imagePath, imageData);
        }

        var cacheJson = JsonConvert.SerializeObject(cacheFile, Formatting.None);
        var metadataFilePath = Path.Combine(tempCacheDir.FullName, MetadataFileName);
        await File.WriteAllTextAsync(metadataFilePath, cacheJson);

        if (File.Exists(CacheArchiveFilePath)) File.Delete(CacheArchiveFilePath);
        ZipFile.CreateFromDirectory(tempCacheDir.FullName, CacheArchiveFilePath);
        tempCacheDir.Delete(true);
    }

    private async Task<CacheFileModel> GetUpdatedCache(CacheFileModel existingCache, string[] installedSabers)
    {
        var cachedMetaPaths = existingCache.CachedMetadata
            .Where(meta => meta.LoaderError == SaberLoaderError.None)
            .ToDictionary(meta => meta.RelativePath);

        var sabersToLoadForCaching = installedSabers.WhereNot(cachedMetaPaths.ContainsKey);
        if (!sabersToLoadForCaching.Any())
        {
            // no new sabers were found, so continue with existing cache
            return existingCache;
        }

        var loadedMetadata = await LoadMetadataFromSabers(sabersToLoadForCaching);
        loadedMetadata.ForEach(m => cachedMetaPaths.Add(m.RelativePath, m));

        return new CacheFileModel(Plugin.Version.ToString(), cachedMetaPaths.Values.ToArray());
    }

    private async Task<List<SaberMetadataModel>> LoadMetadataFromSabers(IEnumerable<string> sabersForCaching)
    {
        var saberMetadata = new List<SaberMetadataModel>();
        var itemsCount = sabersForCaching.Count();
        var lastPercent = 0;
        var currentItem = 0;

        foreach (var saber in sabersForCaching)
        {
            using var saberData = await customSabersLoader.GetSaberData(saber, false);

            saberMetadata.Add(new SaberMetadataModel(
                saberData.Metadata.FileInfo.RelativePath ?? throw new ArgumentNullException(),
                saberData.Metadata.FileInfo.Hash ?? throw new ArgumentNullException(),
                saberData.Metadata.FileInfo.Type,
                saberData.Metadata.LoaderError,
                saberData.Metadata.Descriptor.SaberName.FullName,
                saberData.Metadata.Descriptor.AuthorName.FullName,
                saberData.Metadata.Flags.IncompatibleShaders,
                saberData.Metadata.Flags.IncompatibleShaderNames));

            var currentPercent = (currentItem + 1) * 100 / itemsCount;
            if (currentPercent > lastPercent)
            {
                LoadingProgressChanged?.Invoke(currentPercent);
                lastPercent = currentPercent;
            }
            currentItem++;
        }
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
