using CustomSabersLite.Components.Managers;
using CustomSabersLite.Data;
using System.IO;
using System.Threading.Tasks;

namespace CustomSabersLite.Utilities.AssetBundles;

/// <summary>
/// Class for loading different kinds of custom saber assets
/// </summary>
internal class CustomSabersLoader(SaberInstanceManager saberInstanceManager, SaberLoader saberLoader, WhackerLoader whackerLoader)
{
    private readonly SaberInstanceManager saberInstanceManager = saberInstanceManager;
    private readonly SaberLoader saberLoader = saberLoader;
    private readonly WhackerLoader whackerLoader = whackerLoader;

    public async Task<CustomSaberData> GetSaberData(string saberPath) =>
        string.IsNullOrWhiteSpace(saberPath) ? null
        : saberInstanceManager.TryGetSaber(saberPath) ?? await LoadNew(saberPath);

    public async Task<(CustomSaberData saberData, SaberLoaderError loadingError)> LoadSaberDataAsync(string relativeSaberPath) =>
        SaberAssetBlacklist.IsOnBlacklist(relativeSaberPath) ? (CustomSaberData.Empty, SaberLoaderError.Blacklist)
        : Path.GetExtension(relativeSaberPath) switch
        {
            FileExts.Saber => await saberLoader.LoadCustomSaberAsync(relativeSaberPath),
            FileExts.Whacker => await whackerLoader.LoadWhackerAsync(relativeSaberPath),
            _ => (CustomSaberData.Empty, SaberLoaderError.InvalidFileType)
        };

    private async Task<CustomSaberData> LoadNew(string saberPath)
    {
        var saberData = (await LoadSaberDataAsync(saberPath)).saberData;
        saberInstanceManager.AddSaber(saberData);
        return saberData;
    }
}
