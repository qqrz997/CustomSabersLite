using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;
using CustomSabersLite.Data;
using UnityEngine;
using AssetBundleLoadingTools.Utilities;
using System.Threading.Tasks;

namespace CustomSabersLite.Utilities.AssetBundles;

/// <summary>
/// Class for loading .whacker files
/// </summary>
internal class WhackerLoader(PluginDirs pluginDirs, BundleLoader bundleLoader)
{
    private readonly BundleLoader bundleLoader = bundleLoader;
    private readonly string sabersPath = pluginDirs.CustomSabers.FullName;

    /// <summary>
    /// Loads a custom saber from a .whacker file
    /// </summary>
    /// <param name="relativePath">Path to the .whacker file in the CustomSabers folder</param>
    /// <returns><seealso cref="CustomSaberData.Default"/> if a custom saber failed to load</returns>
    public async Task<CustomSaberData> LoadWhackerAsync(string relativePath)
    {
        var path = Path.Combine(sabersPath, relativePath);
        if (!File.Exists(path))
        {
            return CustomSaberData.Default;
        }

        Logger.Debug($"Attempting to load whacker file...\n\t- {path}");

        using var archive = ZipFile.OpenRead(path);
        var json = archive.Entries.Where(x => x.FullName.EndsWith(".json")).FirstOrDefault();

        using var jsonStream = json.Open();
        using var jsonStreamReader = new StreamReader(jsonStream);
        var whacker = (WhackerObject)new JsonSerializer().Deserialize(jsonStreamReader, typeof(WhackerObject));

        var bundleEntry = archive.GetEntry(whacker.pcFileName);
        var thumbEntry = archive.GetEntry(whacker.descriptor.coverImage);

        using var bundleStream = bundleEntry.Open();
        var bundle = await bundleLoader.LoadBundleAsync(bundleStream);
        if (!bundle)
        {
            return CustomSaberData.Default;
        }
        var saberPrefab = await AssetBundleExtensions.LoadAssetAsync<GameObject>(bundle, "_Whacker");
        if (!saberPrefab)
        {
            bundle.Unload(true);
            return CustomSaberData.Default;
        }
        saberPrefab.hideFlags = HideFlags.DontUnloadUnusedAsset;

        var descriptor = new SaberDescriptor
        {
            SaberName = whacker.descriptor.objectName,
            AuthorName = whacker.descriptor.author,
            Description = whacker.descriptor.description,
            CoverImage = thumbEntry is null ? null : await GetCoverFromArchive(thumbEntry)
        };
        bundle.Unload(false);

        var missingShaders = !await ShaderRepairUtils.RepairSaberShadersAsync(saberPrefab);
        
        return new CustomSaberData(relativePath, saberPrefab, descriptor, CustomSaberType.Whacker) { MissingShaders = missingShaders };
    }

    private async Task<Sprite> GetCoverFromArchive(ZipArchiveEntry thumbEntry)
    {
        using var memoryStream = new MemoryStream();
        await thumbEntry.Open().CopyToAsync(memoryStream);
        return memoryStream.ToArray().LoadImage();
    }
}
