using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using CustomSabersLite.Models;

namespace CustomSabersLite.Utilities.AssetBundles;

/// <summary>
/// Class for loading .saber files
/// </summary>
internal class SaberLoader(PluginDirs dirs, BundleLoader bundleLoader)
{
    private readonly BundleLoader bundleLoader = bundleLoader;
    private readonly string sabersPath = dirs.CustomSabers.FullName;

    private const CustomSaberType Type = CustomSaberType.Saber;

    /// <summary>
    /// Loads a custom saber from a .saber file
    /// </summary>
    /// <param name="relativePath">Path to the .saber file in the CustomSabers folder</param>
    /// <returns><seealso cref="NoSaberData"/> if a custom saber failed to load</returns>
    public async Task<ISaberData> LoadCustomSaberAsync(string relativePath)
    {
        var path = Path.Combine(sabersPath, relativePath);

        if (!File.Exists(path))
            return new NoSaberData(relativePath, SaberLoaderError.FileNotFound);

        Logger.Debug($"Attempting to load saber file...\n\t- {path}");

        var bundle = await bundleLoader.LoadBundleAsync(path);

        if (!bundle)
            return new NoSaberData(relativePath, SaberLoaderError.NullBundle);

        var saberPrefab = await bundleLoader.LoadAssetAsync<GameObject>(bundle, "_CustomSaber");

        if (!saberPrefab)
        {
            bundle.Unload(true);
            return new NoSaberData(relativePath, SaberLoaderError.NullAsset);
        }

        var descriptor = saberPrefab.GetComponent<SaberDescriptor>();
        var image = descriptor.CoverImage?.texture?.DuplicateTexture().Downscale(128, 128).EncodeToPNG();

        saberPrefab.hideFlags |= HideFlags.DontUnloadUnusedAsset;
        saberPrefab.name += $" {descriptor.SaberName}";

        var shaderInfo = await ShaderRepairUtils.RepairSaberShadersAsync(saberPrefab);
        var missingShaders = !shaderInfo.AllShadersReplaced;
        var missingShaderNames = shaderInfo.MissingShaderNames;

        return 
            new CustomSaberData(
                new CustomSaberMetadata(
                    new SaberFileInfo(relativePath, Type),
                    SaberLoaderError.None,
                    new Descriptor(descriptor.SaberName, descriptor.AuthorName, image), 
                    new SaberModelFlags(missingShaders, missingShaderNames)), // todo - missing shader names
                bundle,
                saberPrefab);
    }
}
