using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Services;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CustomSabersLite.Utilities;

/// <summary>
/// Class for loading different kinds of custom saber assets
/// </summary>
internal class CustomSabersLoader(SaberInstanceManager saberInstanceManager, SaberLoader saberLoader, WhackerLoader whackerLoader, ITimeService timeService)
{
    private readonly SaberInstanceManager saberInstanceManager = saberInstanceManager;
    private readonly SaberLoader saberLoader = saberLoader;
    private readonly WhackerLoader whackerLoader = whackerLoader;
    private readonly ITimeService timeService = timeService;

    public async Task<ISaberData> GetSaberData(string saberPath, bool keepSaberInstance) =>
        saberInstanceManager.TryGetSaber(saberPath) ?? await LoadNew(saberPath, keepSaberInstance);

    private async Task<ISaberData> LoadNew(string saberPath, bool keepSaberInstance)
    {
        try
        {
            var saberData = await LoadSaberDataAsync(saberPath);

            if (keepSaberInstance && saberData is CustomSaberData customSaber)
            {
                saberInstanceManager.AddSaber(customSaber);
            }

            return saberData;
        }
        catch (DirectoryNotFoundException)
        {
            return new NoSaberData(saberPath, timeService.GetUtcTime(), SaberLoaderError.FileNotFound);
        }
        catch (Exception ex)
        {
            Logger.Warn($"Problem encountered when trying to load a saber\n{ex}");
        }
        return new NoSaberData(saberPath, timeService.GetUtcTime(), SaberLoaderError.Unknown);
    }

    private async Task<ISaberData> LoadSaberDataAsync(string relativePath) => Path.GetExtension(relativePath) switch
    {
        ".saber" => await saberLoader.LoadCustomSaberAsync(relativePath),
        ".whacker" => await whackerLoader.LoadWhackerAsync(relativePath),
        _ => new NoSaberData(relativePath, timeService.GetUtcTime(), SaberLoaderError.InvalidFileType)
    };
}
