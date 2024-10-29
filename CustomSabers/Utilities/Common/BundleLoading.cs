using AssetBundleLoadingTools.Utilities;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.Utilities.Common;

/// <summary>
/// Uses AssetBundleExtensions to greatly simplify async <seealso cref="AssetBundle"/> loading
/// </summary>
internal class BundleLoading
{
    public static async Task<AssetBundle?> LoadBundle(string path) =>
        await AssetBundleExtensions.LoadFromFileAsync(path);

    public static async Task<AssetBundle?> LoadBundle(Stream stream) =>
        !stream.CanRead || !stream.CanSeek ? await CopyStreamAndLoadBundle(stream)
        : await AssetBundleExtensions.LoadFromStreamAsync(stream);

    private static async Task<AssetBundle?> CopyStreamAndLoadBundle(Stream stream)
    {
        using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        return await AssetBundleExtensions.LoadFromStreamAsync(memoryStream);
    }

    public static async Task<T?> LoadAsset<T>(AssetBundle bundle, string assetPath) where T : Object =>
        await AssetBundleExtensions.LoadAssetAsync<T>(bundle, assetPath);
}
