using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using CustomSabersLite.Data;

namespace CustomSabersLite.Utilities.AssetBundles
{
    /// <summary>
    /// Class for loading .saber files
    /// </summary>
    internal class CustomSaberLoader
    {
        private readonly BundleLoader bundleLoader;
        private readonly string sabersPath;

        public CustomSaberLoader(PluginDirs dirs, BundleLoader bundleLoader)
        {
            this.bundleLoader = bundleLoader;
            sabersPath = dirs.CustomSabers.FullName;
        }

        /// <summary>
        /// Loads a custom saber from a .saber file
        /// </summary>
        /// <param name="relativePath">Path to the .saber file in the CustomSabers folder</param>
        /// <returns><seealso cref="CustomSaberData.ForDefaultSabers"/> if a custom saber failed to load</returns>
        public async Task<CustomSaberData> LoadCustomSaberAsync(string relativePath)
        {
            string path = Path.Combine(sabersPath, relativePath);

            if (!File.Exists(path))
            {
                return CustomSaberData.ForDefaultSabers();
            }

            AssetBundle bundle = await bundleLoader.LoadBundleAsync(path);

            if (bundle is null)
            {
                return CustomSaberData.ForDefaultSabers();
            }

            GameObject saberPrefab = await bundleLoader.LoadAssetAsync<GameObject>(bundle, "_CustomSaber");

            if (saberPrefab is null)
            {
                bundle.Unload(true);
                return CustomSaberData.ForDefaultSabers();
            }
            saberPrefab.hideFlags = HideFlags.DontUnloadUnusedAsset;

            SaberDescriptor descriptor = saberPrefab.GetComponent<SaberDescriptor>();
            bundle.Unload(false);

            bool missingShaders = !await ShaderRepairUtils.RepairSaberShadersAsync(saberPrefab);

            return new CustomSaberData(relativePath, saberPrefab, descriptor, CustomSaberType.Saber) { MissingShaders = missingShaders };
        }
    }
}
