using AssetBundleLoadingTools.Utilities;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.Utilities.AssetBundles
{
    internal class BundleLoader
    {
        public async Task<AssetBundle> LoadBundleAsync(string path) =>
            await AssetBundleExtensions.LoadFromFileAsync(path);

        public async Task<AssetBundle> LoadBundleAsync(Stream stream)
        {
            AssetBundle bundle;

            if (stream.CanRead && stream.CanSeek)
            {
                bundle = await AssetBundleExtensions.LoadFromStreamAsync(stream);
            }
            else
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await stream.CopyToAsync(memoryStream);
                    bundle = await AssetBundleExtensions.LoadFromStreamAsync(memoryStream);
                }
            }

            return bundle;
        }

        public async Task<T> LoadAssetAsync<T>(AssetBundle bundle, string assetPath) where T : Object =>
            await AssetBundleExtensions.LoadAssetAsync<T>(bundle, assetPath);
    }
}
