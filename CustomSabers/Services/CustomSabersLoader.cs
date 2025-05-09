using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CustomSabersLite.Models;

namespace CustomSabersLite.Services;

/// <summary>
/// Class for loading different kinds of custom saber assets
/// </summary>
internal class CustomSabersLoader
{
    private readonly SaberPrefabCache saberPrefabCache;
    private readonly SaberLoader saberLoader;
    private readonly WhackerLoader whackerLoader;

    public CustomSabersLoader(
        SaberPrefabCache saberPrefabCache,
        SaberLoader saberLoader,
        WhackerLoader whackerLoader)
    {
        this.saberPrefabCache = saberPrefabCache;
        this.saberLoader = saberLoader;
        this.whackerLoader = whackerLoader;
    }

    private readonly Dictionary<SaberFileInfo, Task<ISaberData>> runningTasks = [];

    public async Task<ISaberData> GetSaberData(SaberFileInfo saberFile, bool keepSaberInstance, CancellationToken token)
    {
        if (saberPrefabCache.TryGetSaberPrefab(saberFile.Hash, out var cachedSaberData))
        {
            return cachedSaberData;
        }
        
        if (runningTasks.TryGetValue(saberFile, out var task))
        {
            return await task;
        }

        var loadSaberDataTask = LoadSaberDataAsync(saberFile);
        runningTasks.Add(saberFile, loadSaberDataTask);

        try
        {
            var saberData = await loadSaberDataTask;

            if (token.IsCancellationRequested)
            {
                saberData.Dispose();
                throw new OperationCanceledException();
            }

            if (keepSaberInstance && saberData is CustomSaberData customSaber)
            {
                saberPrefabCache.AddSaberPrefab(customSaber);
            }

            return saberData;
        }
        catch (DirectoryNotFoundException)
        {
            return new NoSaberData(saberFile, SaberLoaderError.FileNotFound);
        }
        finally
        {
            runningTasks.Remove(saberFile);
        }
    }

    private async Task<ISaberData> LoadSaberDataAsync(SaberFileInfo saberFile) => saberFile.FileInfo.Extension switch
    {
        ".saber" => await saberLoader.LoadCustomSaberAsync(saberFile),
        ".whacker" => await whackerLoader.LoadWhackerAsync(saberFile),
        _ => new NoSaberData(saberFile, SaberLoaderError.InvalidFileType)
    };
}
