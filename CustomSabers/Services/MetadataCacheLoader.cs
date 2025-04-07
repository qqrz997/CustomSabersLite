using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using Newtonsoft.Json;
using SiraUtil.Zenject;
using UnityEngine;

namespace CustomSabersLite.Services;

internal class MetadataCacheLoader : IAsyncInitializable, IDisposable
{
    private readonly CustomSabersLoader customSabersLoader;
    private readonly SpriteCache spriteCache;
    private readonly SaberPrefabCache saberPrefabCache;
    private readonly SaberMetadataCacheMigrationManager saberMetadataCacheMigrationManager;
    private readonly FileManager fileManager;
    private readonly SaberMetadataCache saberMetadataCache;
    private readonly DirectoryManager directoryManager;
    private readonly SaberMetadataConverter saberMetadataConverter;

    public MetadataCacheLoader(CustomSabersLoader customSabersLoader,
        SpriteCache spriteCache,
        SaberPrefabCache saberPrefabCache,
        SaberMetadataCacheMigrationManager saberMetadataCacheMigrationManager,
        FileManager fileManager, 
        SaberMetadataCache saberMetadataCache, 
        DirectoryManager directoryManager,
        SaberMetadataConverter saberMetadataConverter)
    {
        this.customSabersLoader = customSabersLoader;
        this.spriteCache = spriteCache;
        this.saberPrefabCache = saberPrefabCache;
        this.saberMetadataCacheMigrationManager = saberMetadataCacheMigrationManager;
        this.fileManager = fileManager;
        this.saberMetadataCache = saberMetadataCache;
        this.directoryManager = directoryManager;
        this.saberMetadataConverter = saberMetadataConverter;
    }

    private CancellationTokenSource reloadTokenSource = new();
    private Progress currentProgress = new(string.Empty);

    private string CacheArchiveFilePath => Path.Combine(directoryManager.UserData.FullName, "cache");

    public event Action<Progress>? LoadingProgressChanged;
    
    public Progress CurrentProgress
    {
        get => currentProgress;
        private set
        {
            if (currentProgress == value) return;
            currentProgress = value;
            LoadingProgressChanged?.Invoke(value);
        }
    }

    public async Task InitializeAsync(CancellationToken token)
    {
        await ReloadAsync();
    }

    public async Task ReloadAsync()
    {
        reloadTokenSource.CancelThenDispose();
        reloadTokenSource = new();

        try
        {
            await ReloadAsync(reloadTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            Logger.Debug("Reload operation cancelled.");
        }
        catch (Exception e)
        {
            Logger.Error($"Problem encountered during reload\n{e}");
        }
    }
    
    private async Task ReloadAsync(CancellationToken token) 
    {
        saberPrefabCache.Clear();
        saberMetadataCache.Clear();
        var stopwatch = Stopwatch.StartNew();
        var simpleIntProgress = new Progress<int>(v => CurrentProgress = currentProgress with { StagePercent = v });
        IProgress<Progress> stageChangedProgress = new Progress<Progress>(p => CurrentProgress = p);
        
        stageChangedProgress.Report(new("Retrieving Saber Files"));
        var localSaberFiles = await fileManager.GetSaberFilesAsync(token, simpleIntProgress);
        var installedSaberHashes = localSaberFiles.Select(file => file.Hash).ToHashSet();

        stageChangedProgress.Report(new("Loading Cache"));
        var localCacheFile = await GetLocalCache(token);

        var sabersToLoad = GetSabersToLoad(localCacheFile, localSaberFiles).ToArray();
        Logger.Notice($"Found {sabersToLoad.Length} saber files to load");
        
        stageChangedProgress.Report(new("Loading Sabers"));
        var updatedLocalCache = await GetUpdatedLocalCache(localCacheFile, sabersToLoad, token, simpleIntProgress);
        
        if (localCacheFile != updatedLocalCache)
        {
            stageChangedProgress.Report(new("Saving Metadata"));
            await SaveMetadataToLocalCache(updatedLocalCache);
        }
        
        UpdateMetadataCache(updatedLocalCache, localSaberFiles, installedSaberHashes);

        #if SHADER_DEBUG
        ShaderInfoDump.Instance.DumpTo(DirectoryManager.UserData.FullName);
        #endif

        stopwatch.Stop();
        Logger.Notice($"Cache loading took {stopwatch.ElapsedMilliseconds}ms");

        stageChangedProgress.Report(new("Completed", Completed: true));
    }

    private async Task<CacheFileModel> GetLocalCache(CancellationToken token)
    {
        if (!await saberMetadataCacheMigrationManager.MigrationTask)
        {
            Logger.Warn("Internal reload was denied because of a failure during cache migration");
            return CacheFileModel.Empty;
        }
        token.ThrowIfCancellationRequested();

        var cacheFile = new FileInfo(CacheArchiveFilePath);
        
        if (!cacheFile.Exists) return CacheFileModel.Empty;

        using var cacheZipArchive = ZipFile.OpenRead(cacheFile.FullName);

        var metadataJsonEntry = cacheZipArchive.GetEntry("metadata.json");
        await using var metadataJsonStream = metadataJsonEntry?.Open();

        if (metadataJsonStream is null) return CacheFileModel.Empty;

        var cache = metadataJsonStream.DeserializeStream<CacheFileModel>()?.WithValidation() ?? CacheFileModel.Empty;
        
        foreach (var meta in cache.CachedMetadata)
        {
            token.ThrowIfCancellationRequested();
            var iconEntry = cacheZipArchive.GetEntry($"images/{meta.Hash}.png");
            var sprite = await LoadSpriteFromEntry(iconEntry, token);
            spriteCache.AddSprite(meta.Hash, sprite);
        }

        return cache;
    }

    private async Task SaveMetadataToLocalCache(CacheFileModel cacheFile)
    {
        var tempCacheDir = Directory.CreateDirectory(Path.Combine(directoryManager.UserData.FullName, "temp"));
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
        catch (Exception ex)
        {
            Logger.Warn($"Problem encountered when saving the saber metadata cache\n{ex}");
        }
        finally
        {
            tempCacheDir.Delete(true);
        }
    }
    
    private async Task<CacheFileModel> GetUpdatedLocalCache(
        CacheFileModel existingCache, SaberFileInfo[] sabersToLoad, CancellationToken token, IProgress<int> progress)
    {
        
        if (!sabersToLoad.Any())
        {
            return existingCache; // nothing to update
        }

        var loadedMetadata = await LoadMetadataFromSabers(sabersToLoad, token, progress);
        
        if (!loadedMetadata.Any())
        {
            return existingCache;
        }
        
        // add the new metadata to the existing metadata
        var cachedMetadata = existingCache.CachedMetadata.Concat(loadedMetadata).ToArray();
        
        return new(Plugin.Version.ToString(), cachedMetadata);
    }
    
    private async Task<List<SaberMetadataModel>> LoadMetadataFromSabers(
        IList<SaberFileInfo> sabersForCaching, CancellationToken token, IProgress<int> progress)
    {
        progress.Report(0);
        var loadedSaberMetadata = new List<SaberMetadataModel>();
        int lastPercent = 0;

        for (var i = 0; i < sabersForCaching.Count; i++)
        {
            token.ThrowIfCancellationRequested();

            using var saberData = await customSabersLoader.GetSaberData(sabersForCaching[i], false, token);

            loadedSaberMetadata.Add(saberMetadataConverter.CreateJson(saberData.Metadata));

            int newPercent = (i + 1) * 100 / sabersForCaching.Count;
            if (newPercent != lastPercent)
            {
                progress.Report(newPercent);
                lastPercent = newPercent;
            }
        }

        return loadedSaberMetadata;
    }

    private void UpdateMetadataCache(
        CacheFileModel updatedLocalCache, SaberFileInfo[] localSaberFiles, HashSet<string> installedSaberHashes) => 
        updatedLocalCache.CachedMetadata
            .Join(localSaberFiles,
                saberMetadata => saberMetadata.Hash,
                saberFileInfo => saberFileInfo.Hash,
                (meta, file) => (meta, file))
            .Where(tuple => installedSaberHashes.Contains(tuple.file.Hash))
            .Select(tuple => saberMetadataConverter.ConvertJson(tuple.meta, tuple.file))
            .ForEach(meta => saberMetadataCache.TryAdd(meta));

    private static async Task<Sprite?> LoadSpriteFromEntry(ZipArchiveEntry? entry, CancellationToken token)
    {
        if (entry is null) return null;
        using var ms = new MemoryStream();
        await using var s = entry.Open();
        await s.CopyToAsync(ms, token);
        return new Texture2D(2, 2).ToSprite(ms.ToArray());
    }

    private static IEnumerable<SaberFileInfo> GetSabersToLoad(CacheFileModel existingCache, SaberFileInfo[] localSaberFiles)
    {
        // the cache shouldn't hold duplicate data for the same saber file in different directories
        // we use the hash to make sure we only ever have one of any potential duplicate saber files
        var cachedSaberHashes = existingCache.CachedMetadata.Select(meta => meta.Hash).ToHashSet();
        return localSaberFiles.Where(file => !cachedSaberHashes.Contains(file.Hash));
    }

    public void Dispose()
    {
        reloadTokenSource.CancelThenDispose();
    }

    internal record Progress(string Stage, bool Completed = false, int? StagePercent = null);
}
