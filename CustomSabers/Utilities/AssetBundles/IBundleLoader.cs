using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.Utilities.AssetBundles
{
    internal interface IBundleLoader
    {
        /// <summary>
        /// Loads a <see cref="AssetBundle"/> utilising AssetBundleLoadingTools
        /// </summary>
        /// <param name="path">Absolute path to the bundle on disk</param>
        Task<AssetBundle> LoadBundleAsync(string path);
        Task<AssetBundle> LoadBundleAsync(Stream stream);

        /// <summary>
        /// Loads an asset from an <see cref="AssetBundle"/> using AssetBundleLoadingTools
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bundle">Bundle to load from</param>
        /// <param name="path">Path to the asset in the bundle</param>
        Task<T> LoadAssetAsync<T>(AssetBundle bundle, string path) where T : Object;
    }
}
