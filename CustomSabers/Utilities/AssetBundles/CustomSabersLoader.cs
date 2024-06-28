using CustomSabersLite.Components.Managers;
using CustomSabersLite.Data;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace CustomSabersLite.Utilities.AssetBundles;

/// <summary>
/// Class for loading different kinds of custom saber assets
/// </summary>
internal class CustomSabersLoader(SaberInstanceManager saberInstanceManager, SaberLoader saberLoader, WhackerLoader whackerLoader)
{
    private readonly SaberInstanceManager saberInstanceManager = saberInstanceManager;
    private readonly SaberLoader saberLoader = saberLoader;
    private readonly WhackerLoader whackerLoader = whackerLoader;

    public async Task<CustomSaberData> GetSaberData(string saberPath)
    {
        if (!saberInstanceManager.TryGetSaber(saberPath, out var saberData))
        {
            saberData = await LoadSaberDataAsync(saberPath);
            saberInstanceManager.AddSaber(saberData);
        }
        return saberData;
    }

    public async Task<CustomSaberData> LoadSaberDataAsync(string relativeSaberPath) => Path.GetExtension(relativeSaberPath) switch
    {
        FileExts.Saber => await saberLoader.LoadCustomSaberAsync(relativeSaberPath),
        FileExts.Whacker => await whackerLoader.LoadWhackerAsync(relativeSaberPath),
        _ => CustomSaberData.Default
    };
}
