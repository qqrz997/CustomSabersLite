using CustomSabersLite.Components.Managers;
using CustomSabersLite.Models;
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

    public async Task<ISaberData> GetSaberData(string saberPath) =>
        string.IsNullOrWhiteSpace(saberPath) ? null
        : saberInstanceManager.TryGetSaber(saberPath) ?? await LoadNew(saberPath);

    public async Task<ISaberData> LoadSaberDataAsync(string relativePath) => Path.GetExtension(relativePath) switch
    {
        FileExts.Saber => await saberLoader.LoadCustomSaberAsync(relativePath),
        FileExts.Whacker => await whackerLoader.LoadWhackerAsync(relativePath),
        _ => new NoSaberData(relativePath, SaberLoaderError.InvalidFileType)
    };

    private async Task<ISaberData> LoadNew(string saberPath)
    {
        var saberData = await LoadSaberDataAsync(saberPath);
        if (saberData is CustomSaberData customSaber)
        {
            saberInstanceManager.AddSaber(customSaber);
        }
        return saberData;
    }
}
