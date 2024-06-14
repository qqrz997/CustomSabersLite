using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using CustomSabersLite.Data;
using System;

namespace CustomSabersLite.Utilities.AssetBundles
{
    /// <summary>
    /// Class for loading .saber files
    /// </summary>
    internal class SaberLoader
    {
        private readonly BundleLoader bundleLoader;
        private readonly string sabersPath;

        public SaberLoader(PluginDirs dirs, BundleLoader bundleLoader)
        {
            this.bundleLoader = bundleLoader;
            sabersPath = dirs.CustomSabers.FullName;
        }

        /// <summary>
        /// Loads a custom saber from a .saber file
        /// </summary>
        /// <param name="relativePath">Path to the .saber file in the CustomSabers folder</param>
        /// <returns><seealso cref="CustomSaberData.Default"/> if a custom saber failed to load</returns>
        public async Task<CustomSaberData> LoadCustomSaberAsync(string relativePath)
        {
            string path = Path.Combine(sabersPath, relativePath);

            if (!File.Exists(path))
            {
                return CustomSaberData.Default;
            }

            AssetBundle bundle = await bundleLoader.LoadBundleAsync(path);

            if (bundle == null)
            {
                return CustomSaberData.Default;
            }

            GameObject saberPrefab = await bundleLoader.LoadAssetAsync<GameObject>(bundle, "_CustomSaber");

            if (saberPrefab == null)
            {
                bundle.Unload(true);
                return CustomSaberData.Default;
            }
            saberPrefab.hideFlags = HideFlags.DontUnloadUnusedAsset;

            SaberDescriptor descriptor = saberPrefab.GetComponent<SaberDescriptor>();
            bundle.Unload(false);

            bool missingShaders = !await ShaderRepairUtils.RepairSaberShadersAsync(saberPrefab);

            return new CustomSaberData(relativePath, saberPrefab, descriptor, CustomSaberType.Saber) { MissingShaders = missingShaders };
        }
    }
}
