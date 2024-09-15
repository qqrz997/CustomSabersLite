using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;
using CustomSabersLite.Models;
using UnityEngine;
using AssetBundleLoadingTools.Utilities;
using System.Threading.Tasks;

namespace CustomSabersLite.Utilities.AssetBundles;

/// <summary>
/// Class for loading .whacker files
/// </summary>
internal class WhackerLoader(PluginDirs directories, BundleLoader bundleLoader)
{
    private readonly BundleLoader bundleLoader = bundleLoader;
    private readonly string sabersPath = directories.CustomSabers.FullName;

    private const CustomSaberType Type = CustomSaberType.Whacker;

    /// <summary>
    /// Loads a custom saber from a .whacker file
    /// </summary>
    /// <param name="relativePath">Path to the .whacker file in the CustomSabers folder</param>
    /// <returns><seealso cref="NoSaberData"/> if a custom saber failed to load</returns>
    public async Task<ISaberData> LoadWhackerAsync(string relativePath)
    {
        var path = Path.Combine(sabersPath, relativePath);

        if (!File.Exists(path))
            return new NoSaberData(relativePath, SaberLoaderError.FileNotFound);

        Logger.Debug($"Attempting to load whacker file...\n\t- {path}");

        using var archive = ZipFile.OpenRead(path);
        var json = archive.Entries.Where(x => x.FullName.EndsWith(".json")).FirstOrDefault();

        using var jsonStream = json.Open();
        using var jsonStreamReader = new StreamReader(jsonStream);
        var whacker = (WhackerModel)new JsonSerializer().Deserialize(jsonStreamReader, typeof(WhackerModel));

        if (whacker.config.isLegacy)
            return new NoSaberData(relativePath, SaberLoaderError.LegacyWhacker);

        var bundleEntry = archive.GetEntry(whacker.pcFileName);

        var thumbEntry = archive.GetEntry(whacker.descriptor.coverImage);

        using var bundleStream = bundleEntry.Open();
        var bundle = await bundleLoader.LoadBundleAsync(bundleStream);

        if (!bundle)
            return new NoSaberData(relativePath, SaberLoaderError.NullBundle);

        var saberPrefab = await AssetBundleExtensions.LoadAssetAsync<GameObject>(bundle, "_Whacker");

        if (!saberPrefab)
        {
            bundle.Unload(true);
            return new NoSaberData(relativePath, SaberLoaderError.NullAsset);
        }

        saberPrefab.hideFlags |= HideFlags.DontUnloadUnusedAsset;
        saberPrefab.name += $" {whacker.descriptor.objectName}";

        byte[] image = null;
        if (thumbEntry != null)
        {
            using var imageMemoryStream = new MemoryStream();
            using var imageStream = thumbEntry.Open();
            await imageStream.CopyToAsync(imageMemoryStream);
            image = imageMemoryStream.ToArray();
        }

        var shaderInfo = await ShaderRepairUtils.RepairSaberShadersAsync(saberPrefab);
        var missingShaders = !shaderInfo.AllShadersReplaced;
        var missingShaderNames = shaderInfo.MissingShaderNames;

        return
            new CustomSaberData(
                new CustomSaberMetadata(
                    new SaberFileInfo(relativePath, Type),
                    SaberLoaderError.None,
                    new Descriptor(whacker.descriptor.objectName, whacker.descriptor.author, image),
                    new SaberModelFlags(missingShaders, missingShaderNames)),
                bundle,
                saberPrefab);
    }
}
