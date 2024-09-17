﻿using CustomSabersLite.Models;
using CustomSabersLite.UI.Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Zenject;

namespace CustomSabersLite.Utilities.AssetBundles;

internal class CacheManager : IInitializable
{
    private readonly PluginDirs directories;
    private readonly CustomSabersLoader customSabersLoader;
    private readonly SaberListManager saberListManager;

    private CacheManager(PluginDirs directories, CustomSabersLoader customSabersLoader, SaberListManager saberListManager)
    {
        this.directories = directories;
        this.customSabersLoader = customSabersLoader;
        this.saberListManager = saberListManager;
    }

    private string CacheFilePath => 
        Path.Combine(directories.UserData.FullName, "sabers.cache");

    private string[] InstalledSaberRelativePaths =>
        FileUtils.GetFilePaths(directories.CustomSabers.FullName, [FileExts.Saber, FileExts.Whacker], SearchOption.AllDirectories, true);

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
        Logger.Debug("Initializing caching step");
        saberListManager.SetData([]);

        var existingCache = 
            !File.Exists(CacheFilePath) ? CacheFileModel.CreateNew()
            : JsonConvert.DeserializeObject<CacheFileModel>(await File.ReadAllTextAsync(CacheFilePath)) ?? CacheFileModel.CreateNew();

        // { if the cache format changes the old one should be deleted }

        var metadata = await UpdateAndGetCachedMetadata(existingCache);

        saberListManager.SetData(metadata.Select(m => 
            new CustomSaberMetadata(
                new SaberFileInfo(m.RelativePath, m.SaberType),
                m.LoaderError,
                new Descriptor(m.SaberName, m.AuthorName, m.Image),
                new SaberModelFlags(m.IncompatibleShaders, m.IncompatibleShaderNames))));
    }

    private async Task<IEnumerable<SaberMetadataModel>> UpdateAndGetCachedMetadata(CacheFileModel existingCache)
    {
        var installedSabers = InstalledSaberRelativePaths;
        var cachedMetaPaths = existingCache.CachedMetadata.ToDictionary(meta => meta.RelativePath);

        var sabersToLoadForCaching = installedSabers.WhereNot(cachedMetaPaths.ContainsKey);
        if (!sabersToLoadForCaching.Any())
        {
            // no new sabers were found, so continue with existing cache
            return existingCache.CachedMetadata.Where(meta => installedSabers.Contains(meta.RelativePath));
        }

        var loadedMetadata = await LoadMetadata(sabersToLoadForCaching);
        loadedMetadata.ForEach(m => cachedMetaPaths.Add(m.RelativePath, m));

        var newCache = new CacheFileModel(Plugin.Version.ToString(), cachedMetaPaths.Values.ToArray());

        var cacheJson = JsonConvert.SerializeObject(newCache);
        await File.WriteAllTextAsync(CacheFilePath, cacheJson);

        return newCache.CachedMetadata.Where(meta => installedSabers.Contains(meta.RelativePath));
    }

    private async Task<List<SaberMetadataModel>> LoadMetadata(IEnumerable<string> sabersForCaching)
    {
        var saberMetadata = new List<SaberMetadataModel>();
        var sabersCount = sabersForCaching.Count();
        var lastPercent = 0;
        LoadingProgressChanged?.Invoke(lastPercent);

        for (var i = 0; i < sabersCount; i++)
        {
            var saber = sabersForCaching.ElementAt(i);
            using var saberData = await customSabersLoader.LoadSaberDataAsync(saber);

            saberMetadata.Add(new SaberMetadataModel(
                saberData.Metadata.FileInfo.RelativePath,
                saberData.Metadata.FileInfo.Type,
                saberData.Metadata.LoaderError,
                saberData.Metadata.Descriptor.SaberName.FullName,
                saberData.Metadata.Descriptor.AuthorName.FullName,
                saberData.Metadata.Descriptor.Image,
                saberData.Metadata.Flags.IncompatibleShaders,
                saberData.Metadata.Flags.IncompatibleShaderNames));

            var currentPercent = (i + 1) * 100 / sabersCount;
            if (currentPercent > lastPercent)
            {
                LoadingProgressChanged?.Invoke(currentPercent);
                lastPercent = currentPercent;
            }
        }
        return saberMetadata;
    }
}
