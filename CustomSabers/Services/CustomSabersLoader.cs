using System;
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

    public async Task<ISaberData> GetSaberData(
        SaberFileInfo saberFile,
        bool keepSaberInstance,
        CancellationToken token) => 
        saberPrefabCache.TryGetSaberPrefab(saberFile.Hash, out var saberData) ? saberData 
            : await LoadNew(saberFile, keepSaberInstance, token);

    private async Task<ISaberData> LoadNew(SaberFileInfo saberFile, bool keepSaberInstance, CancellationToken token)
    {
        try
        {
            var saberData = await LoadSaberDataAsync(saberFile);

            if (token.IsCancellationRequested)
            {
                // todo: while unlikely, it should be possible that a saber is loaded twice before this dispose 
                saberData.Dispose();
                throw new OperationCanceledException();
            }

            if (keepSaberInstance && saberData is CustomSaberData customSaber)
            {
                saberPrefabCache.AddSaberPrefab(customSaber);
            }

            return saberData;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (DirectoryNotFoundException)
        {
            return new NoSaberData(saberFile, SaberLoaderError.FileNotFound);
        }
        catch (Exception ex)
        {
            Logger.Warn($"Problem encountered when trying to load a saber\n{ex}");
        }
        return new NoSaberData(saberFile, SaberLoaderError.Unknown);
    }

    private async Task<ISaberData> LoadSaberDataAsync(SaberFileInfo saberFile) => saberFile.FileInfo.Extension switch
    {
        ".saber" => await saberLoader.LoadCustomSaberAsync(saberFile),
        ".whacker" => await whackerLoader.LoadWhackerAsync(saberFile),
        _ => new NoSaberData(saberFile, SaberLoaderError.InvalidFileType)
    };
}
