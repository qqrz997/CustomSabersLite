using AssetBundleLoadingTools.Utilities;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.Utilities.AssetBundles
{
    /// <summary>
    /// Uses AssetBundleExtensions to greatly simplify async <seealso cref="AssetBundle"/> loading
    /// </summary>
    internal class BundleLoader
    {
        public async Task<AssetBundle> LoadBundleAsync(string path) =>
            await AssetBundleExtensions.LoadFromFileAsync(path);

        public async Task<AssetBundle> LoadBundleAsync(Stream stream) =>
            stream.CanRead && stream.CanSeek
            ? await AssetBundleExtensions.LoadFromStreamAsync(stream)
            : await CopyStreamAndLoadBundle(stream);

        private static async Task<AssetBundle> CopyStreamAndLoadBundle(Stream stream)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await stream.CopyToAsync(memoryStream);
                return await AssetBundleExtensions.LoadFromStreamAsync(memoryStream);
            }
        }

        public async Task<T> LoadAssetAsync<T>(AssetBundle bundle, string assetPath) where T : Object =>
            await AssetBundleExtensions.LoadAssetAsync<T>(bundle, assetPath);
    }
}
