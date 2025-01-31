using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;
using static CustomSabersLite.Utilities.Common.JsonReading;

namespace CustomSabersLite.Utilities.Services;

internal class MetadataCacheLoader : IInitializable
{
    private readonly CustomSabersLoader customSabersLoader;
    private readonly SpriteCache spriteCache;
    private readonly SaberPrefabCache saberPrefabCache;
    private readonly SaberMetadataCacheMigrationManager saberMetadataCacheMigrationManager;
    private readonly FileManager fileManager;
    private readonly SaberMetadataCache saberMetadataCache;
    
    public MetadataCacheLoader(CustomSabersLoader customSabersLoader,
        SpriteCache spriteCache,
        SaberPrefabCache saberPrefabCache,
        SaberMetadataCacheMigrationManager saberMetadataCacheMigrationManager,
        FileManager fileManager, 
        SaberMetadataCache saberMetadataCache)
    {
        this.customSabersLoader = customSabersLoader;
        this.spriteCache = spriteCache;
        this.saberPrefabCache = saberPrefabCache;
        this.saberMetadataCacheMigrationManager = saberMetadataCacheMigrationManager;
        this.fileManager = fileManager;
        this.saberMetadataCache = saberMetadataCache;
    }

    private static string CacheArchiveFilePath => Path.Combine(PluginDirs.UserData.FullName, "cache");

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
        saberPrefabCache.Clear(false);
        var stopwatch = Stopwatch.StartNew();
        
        CurrentProgress = new(false, "Retrieving Saber Files");
        var localSaberFiles = await fileManager.GetSaberFilesAsync();
        var cacheFile = await ReloadCache(localSaberFiles);
        
        var installedSaberHashes = localSaberFiles.Select(file => file.Hash).ToHashSet();

        var customSaberMetadata = cacheFile.CachedMetadata
            .Where(meta => installedSaberHashes.Contains(meta.Hash))
            .Select(meta => meta.ToSaberMetadata(spriteCache.GetSprite(meta.Hash)));

        #if SHADER_DEBUG
        ShaderInfoDump.Instance.DumpTo(PluginDirs.UserData.FullName);
        #endif

        customSaberMetadata.ForEach(m => saberMetadataCache.TryAdd(m));
        
        stopwatch.Stop();
        Logger.Notice($"Cache loading took {stopwatch.ElapsedMilliseconds}ms");

        CurrentProgress = new(true, "Completed");
    }

    private async Task<CacheFileModel> ReloadCache(SaberFileInfo[] localSaberFiles)
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
        var updatedCacheFile = await UpdateLocalCache(localCacheFile, localSaberFiles);

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

        var metadataJsonEntry = cacheZipArchive.GetEntry("metadata.json");
        using var metadataJsonStream = metadataJsonEntry?.Open();

        if (metadataJsonStream is null) return CacheFileModel.Empty;

        var cache = DeserializeStream<CacheFileModel>(metadataJsonStream)?.WithValidation() ?? CacheFileModel.Empty;
        
        foreach (var meta in cache.CachedMetadata)
        {
            var sprite = await LoadSpriteFromEntry(cacheZipArchive.GetEntry($"images/{meta.Hash}.png"));
            spriteCache.AddSprite(meta.Hash, sprite);
        }

        return cache;
    }

    private async Task SaveMetadataToCache(CacheFileModel cacheFile)
    {
        var tempCacheDir = Directory.CreateDirectory(Path.Combine(PluginDirs.UserData.FullName, "temp"));
        var imagesDir = tempCacheDir.CreateSubdirectory("images");

        try
        {
            foreach (var meta in cacheFile.CachedMetadata)
            {
                var imageData = spriteCache.GetSprite(meta.Hash)?.texture.EncodeToPNG();
                if (imageData == null) continue;

                string imagePath = Path.Combine(imagesDir.FullName, meta.Hash + ".png");
                await File.WriteAllBytesAsync(imagePath, imageData);
            }

            string cacheJson = JsonConvert.SerializeObject(cacheFile, Formatting.None);
            string metadataFilePath = Path.Combine(tempCacheDir.FullName, "metadata.json");
            await File.WriteAllTextAsync(metadataFilePath, cacheJson);

            if (File.Exists(CacheArchiveFilePath)) File.Delete(CacheArchiveFilePath);
            ZipFile.CreateFromDirectory(tempCacheDir.FullName, CacheArchiveFilePath);
        }
        finally
        {
            tempCacheDir.Delete(true);
        }
    }

    private async Task<CacheFileModel> UpdateLocalCache(CacheFileModel existingCache, SaberFileInfo[] localSaberFiles)
    {
        // file paths should always be distinct and are used as a key
        var cachedRelativePaths = existingCache.CachedMetadata.Select(m => m.RelativePath).ToHashSet();

        var notCachedSabers = localSaberFiles
            .Where(file => !cachedRelativePaths.Contains(file.RelativePath))
            .ToList();

        var loadedMetadata = await LoadMetadataFromSabers(notCachedSabers);
        
        if (!loadedMetadata.Any())
        {
            return existingCache; // no new sabers were found, so continue with existing cache
        }

        var cachedMetadata = existingCache.CachedMetadata.Concat(loadedMetadata).ToArray();
        
        return new(Plugin.Version.ToString(), cachedMetadata);
    }
    
    private async Task<List<SaberMetadataModel>> LoadMetadataFromSabers(IList<SaberFileInfo> sabersForCaching)
    {
        CurrentProgress = currentProgress with { Stage = "Loading Sabers" };

        var loadedSaberMetadata = new List<SaberMetadataModel>();
        int currentItem = 1;

        foreach (var saberFile in sabersForCaching)
        {
            using var saberData = await customSabersLoader.GetSaberData(saberFile, false);

            if (saberData.Metadata is CustomSaberMetadata customSaberMetadata)
            {
                loadedSaberMetadata.Add(customSaberMetadata.ToJson());
            }

            CurrentProgress = currentProgress with { StagePercent = currentItem * 100 / sabersForCaching.Count };
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
