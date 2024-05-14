using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using AssetBundleLoadingTools.Utilities;
using CustomSaber;
using CustomSabersLite.Data;
using CustomSabersLite.Managers;

namespace CustomSabersLite.Utilities.AssetBundles
{
    /// <summary>
    /// Class for loading .saber files
    /// </summary>
    internal class CustomSaberLoader : ICustomSaberLoader
    {
        private readonly PluginDirs dirs;
        private readonly IBundleLoader bundleLoader;

        public CustomSaberLoader(PluginDirs dirs, IBundleLoader bundleLoader)
        {
            this.dirs = dirs;
            this.bundleLoader = bundleLoader;
        }

        private readonly Dictionary<string, string> loadedBundles = new Dictionary<string, string>();

        private string SabersPath => dirs.CustomSabers.FullName;

        private string SaberFileExtension => ".saber";

        /// <summary>
        /// Loads the Custom Saber prefab from a .saber file
        /// </summary>
        /// <param name="relativePath">Path to the .saber file</param>
        public async Task<CustomSaberData> LoadCustomSaberAsync(string relativePath)
        {
            if (relativePath.Equals("Default"))
            {
                return new CustomSaberData().ForDefaultSabers();
            }

            if (!Path.HasExtension(relativePath))
            {
                // might remove this
                relativePath += SaberFileExtension;
            }
            else if (Path.GetExtension(relativePath) != SaberFileExtension)
            {
                Logger.Error($"This file is not a .saber");
                return new CustomSaberData().ForDefaultSabers();
            }

            string fileName = Path.GetFileName(relativePath);
            loadedBundles.Add(fileName, relativePath);

            AssetBundle bundle = await bundleLoader.LoadBundleAsync(Path.Combine(SabersPath, relativePath));

            if (bundle is null)
            {
                Logger.Error($"Couldn't load file");
                UnloadBundle(fileName, bundle, true);
                return new CustomSaberData().ForDefaultSabers();
            }

            GameObject sabers = await bundleLoader.LoadAssetAsync<GameObject>(bundle, "_CustomSaber");

            if (sabers is null)
            {
                Logger.Error($"Couldn't load sabers from bundle");
                UnloadBundle(fileName, bundle, true);
                return new CustomSaberData().ForDefaultSabers();
            }

            SaberDescriptor descriptor = sabers.GetComponent<SaberDescriptor>();
            UnloadBundle(fileName, bundle, false);

            List<Material> materials = ShaderRepair.GetMaterialsFromGameObjectRenderers(sabers);
            foreach (Material trailMaterial in sabers.GetComponentsInChildren<CustomTrail>().Select(t => t.TrailMaterial))
            {
                if (!materials.Contains(trailMaterial))
                {
                    materials.Add(trailMaterial);
                }
            }
            await ShaderRepair.FixShadersOnMaterialsAsync(materials);

            return new CustomSaberData(relativePath, sabers, descriptor);
        }

        private void UnloadBundle(string fileName, AssetBundle bundle, bool unloadAllLoadedObjects)
        {
            loadedBundles.Remove(fileName);
            bundle.Unload(unloadAllLoadedObjects);
        }
    }
}
