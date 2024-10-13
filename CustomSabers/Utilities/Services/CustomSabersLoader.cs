using CustomSabersLite.Components.Managers;
using CustomSabersLite.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace CustomSabersLite.Utilities;

/// <summary>
/// Class for loading different kinds of custom saber assets
/// </summary>
internal class CustomSabersLoader(SaberInstanceManager saberInstanceManager, SaberLoader saberLoader, WhackerLoader whackerLoader)
{
    private readonly SaberInstanceManager saberInstanceManager = saberInstanceManager;
    private readonly SaberLoader saberLoader = saberLoader;
    private readonly WhackerLoader whackerLoader = whackerLoader;

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
            return new NoSaberData(saberPath, SaberLoaderError.FileNotFound);
        }
        catch (Exception ex)
        {
            Logger.Warn($"Problem encountered when trying to load a saber\n{ex}");
        }
        return new NoSaberData(saberPath, SaberLoaderError.Unknown);
    }

    private async Task<ISaberData> LoadSaberDataAsync(string relativePath) => Path.GetExtension(relativePath) switch
    {
        FileExts.Saber => await saberLoader.LoadCustomSaberAsync(relativePath),
        FileExts.Whacker => await whackerLoader.LoadWhackerAsync(relativePath),
        _ => new NoSaberData(relativePath, SaberLoaderError.InvalidFileType)
    };
}
