using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using CustomSabersLite.Data;

namespace CustomSabersLite.Utilities.AssetBundles
{
    /// <summary>
    /// Class for loading .saber files
    /// </summary>
    internal class CustomSaberLoader : ICustomSaberLoader
    {
        private readonly IBundleLoader bundleLoader;
        private readonly string sabersPath;

        public CustomSaberLoader(PluginDirs dirs, IBundleLoader bundleLoader)
        {
            this.bundleLoader = bundleLoader;
            sabersPath = dirs.CustomSabers.FullName;
        }

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

            await ShaderRepairUtils.RepairSaberShadersAsync(saberPrefab);

            return new CustomSaberData(relativePath, saberPrefab, descriptor, CustomSaberType.Saber);
        }
    }
}
