using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using CustomSabersLite.Data;

namespace CustomSabersLite.Utilities.AssetBundles;

/// <summary>
/// Class for loading .saber files
/// </summary>
internal class SaberLoader(PluginDirs dirs, BundleLoader bundleLoader)
{
    private readonly BundleLoader bundleLoader = bundleLoader;
    private readonly string sabersPath = dirs.CustomSabers.FullName;

    /// <summary>
    /// Loads a custom saber from a .saber file
    /// </summary>
    /// <param name="relativePath">Path to the .saber file in the CustomSabers folder</param>
    /// <returns><seealso cref="CustomSaberData.Default"/> if a custom saber failed to load</returns>
    public async Task<CustomSaberData> LoadCustomSaberAsync(string relativePath)
    {
        var path = Path.Combine(sabersPath, relativePath);

        if (!File.Exists(path))
        {
            return CustomSaberData.Default;
        }

        Logger.Debug($"Attempting to load saber file...\n\t- {path}");

        var bundle = await bundleLoader.LoadBundleAsync(path);

        if (!bundle)
        {
            return CustomSaberData.Default;
        }

        var saberPrefab = await bundleLoader.LoadAssetAsync<GameObject>(bundle, "_CustomSaber");

        if (!saberPrefab)
        {
            bundle.Unload(true);
            return CustomSaberData.Default;
        }
        saberPrefab.hideFlags = HideFlags.DontUnloadUnusedAsset;

        var descriptor = saberPrefab.GetComponent<SaberDescriptor>();
        bundle.Unload(false);

        var missingShaders = !await ShaderRepairUtils.RepairSaberShadersAsync(saberPrefab);

        return new CustomSaberData(relativePath, saberPrefab, descriptor, CustomSaberType.Saber) { MissingShaders = missingShaders };
    }
}
