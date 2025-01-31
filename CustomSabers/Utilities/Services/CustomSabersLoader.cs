using System;
using System.IO;
using System.Threading.Tasks;
using CustomSabersLite.Models;

namespace CustomSabersLite.Utilities.Services;

/// <summary>
/// Class for loading different kinds of custom saber assets
/// </summary>
internal class CustomSabersLoader
{
    private readonly SaberPrefabCache saberPrefabCache;
    private readonly SaberLoader saberLoader;
    private readonly WhackerLoader whackerLoader;
    private readonly ITimeService timeService;

    public CustomSabersLoader(SaberPrefabCache saberPrefabCache, SaberLoader saberLoader, WhackerLoader whackerLoader, ITimeService timeService)
    {
        this.saberPrefabCache = saberPrefabCache;
        this.saberLoader = saberLoader;
        this.whackerLoader = whackerLoader;
        this.timeService = timeService;
    }

    public async Task<ISaberData> GetSaberData(SaberFileInfo saberFile, bool keepSaberInstance) =>
        saberPrefabCache.TryGetSaberPrefab(saberFile.Hash, out var saberData) ? saberData 
            : await LoadNew(saberFile, keepSaberInstance);

    private async Task<ISaberData> LoadNew(SaberFileInfo saberFile, bool keepSaberInstance)
    {
        try
        {
            var saberData = await LoadSaberDataAsync(saberFile);

            if (keepSaberInstance && saberData is CustomSaberData customSaber)
            {
                saberPrefabCache.AddSaberPrefab(customSaber);
            }

            return saberData;
        }
        catch (DirectoryNotFoundException)
        {
            return new NoSaberData(saberFile.FileInfo.FullName, timeService.GetUtcTime(), SaberLoaderError.FileNotFound);
        }
        catch (Exception ex)
        {
            Logger.Warn($"Problem encountered when trying to load a saber\n{ex}");
        }
        return new NoSaberData(saberFile.FileInfo.FullName, timeService.GetUtcTime(), SaberLoaderError.Unknown);
    }

    private async Task<ISaberData> LoadSaberDataAsync(SaberFileInfo saberFile) => saberFile.FileInfo.Extension switch
    {
        ".saber" => await saberLoader.LoadCustomSaberAsync(saberFile),
        ".whacker" => await whackerLoader.LoadWhackerAsync(saberFile),
        _ => new NoSaberData(saberFile.RelativePath, timeService.GetUtcTime(), SaberLoaderError.InvalidFileType)
    };
}
