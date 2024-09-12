﻿using System.IO;
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
    /// <returns><seealso cref="CustomSaberData.Empty"/> if a custom saber failed to load</returns>
    public async Task<(CustomSaberData, SaberLoaderError)> LoadCustomSaberAsync(string relativePath)
    {
        var path = Path.Combine(sabersPath, relativePath);

        if (!File.Exists(path))
            return (CustomSaberData.Empty, SaberLoaderError.FileNotFound);

        Logger.Debug($"Attempting to load saber file...\n\t- {path}");

        var bundle = await bundleLoader.LoadBundleAsync(path);

        if (!bundle)
            return (CustomSaberData.Empty, SaberLoaderError.NullBundle);

        var saberPrefab = await bundleLoader.LoadAssetAsync<GameObject>(bundle, "_CustomSaber");

        if (!saberPrefab)
        {
            bundle.Unload(true);
            return (CustomSaberData.Empty, SaberLoaderError.NullAsset);
        }

        saberPrefab.hideFlags = HideFlags.DontUnloadUnusedAsset;

        var descriptor = saberPrefab.GetComponent<SaberDescriptor>();
        saberPrefab.name += $" {descriptor.SaberName}";

        var missingShaders = !await ShaderRepairUtils.RepairSaberShadersAsync(saberPrefab);

        return (new CustomSaberData(relativePath, bundle, saberPrefab, descriptor, Type) { MissingShaders = missingShaders },
                SaberLoaderError.None);
    }
}
