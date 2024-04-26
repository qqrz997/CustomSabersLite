using AssetBundleLoadingTools.Utilities;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.Utilities.AssetBundles
{
    internal class BundleLoader : IBundleLoader
    {
        public async Task<AssetBundle> LoadBundleAsync(string path)
        {
            if (!File.Exists(path))
            {
                Logger.Error($"Cannot load bundle. Provided path doesn't lead to a file. Provided file name: {Path.Combine(Directory.GetParent(path).Name, Path.GetFileName(path))}");
                return null;
            }

            AssetBundle bundle = await AssetBundleExtensions.LoadFromMemoryAsync(await File.ReadAllBytesAsync(path));

            if (bundle is null)
            {
                Logger.Error($"Couldn't load bundle from {path}");
                return null;
            }

            return bundle;
        }

        public async Task<T> LoadAssetAsync<T>(AssetBundle bundle, string assetPath) where T : Object
        {
            return await AssetBundleExtensions.LoadAssetAsync<T>(bundle, assetPath);
        }
    }
}
