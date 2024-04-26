using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using AssetBundleLoadingTools.Utilities;
using CustomSaber;

namespace CustomSabersLite.Utilities.AssetBundles
{
    /// <summary>
    /// Class for loading .saber files
    /// </summary>
    internal class CustomSaberLoader
    {
        private readonly PluginDirs dirs;
        private readonly IBundleLoader bundleLoader;

        public CustomSaberLoader(PluginDirs dirs, IBundleLoader bundleLoader)
        {
            this.dirs = dirs;
            this.bundleLoader = bundleLoader;
        }

        private string SabersPath => dirs.CustomSabers.FullName;

        private string SaberFileExtension => ".saber";

        /// <summary>
        /// Loads the Custom Saber prefab from a .saber file
        /// </summary>
        /// <param name="saberFileName">Path to the .saber file</param>
        public async Task<GameObject> LoadCustomSaberAsync(string saberFileName)
        {
            Stopwatch sw = Stopwatch.StartNew();

            Logger.Info($"Trying to load saber\n" +
                        $"{saberFileName}");

            if (!Path.HasExtension(saberFileName))
            {
                // might remove this
                saberFileName += SaberFileExtension;
            }
            else if (Path.GetExtension(saberFileName) != SaberFileExtension)
            {
                Logger.Error($"This file is not a .saber");
                return null;
            }

            AssetBundle bundle = await bundleLoader.LoadBundleAsync(Path.Combine(SabersPath, saberFileName));

            if (bundle is null)
            {
                Logger.Error($"Couldn't load file");
                return null;
            }

            GameObject sabers = await bundleLoader.LoadAssetAsync<GameObject>(bundle, "_CustomSaber");

            if (sabers is null)
            {
                Logger.Error($"Couldn't load sabers from bundle");
                return null;
            }

            SaberDescriptor descriptor = sabers.GetComponent<SaberDescriptor>();

            sw.Stop();
            Logger.Info($"Loaded saber {descriptor?.SaberName ?? saberFileName} in {sw.ElapsedMilliseconds}ms");

            List<Material> materials = ShaderRepair.GetMaterialsFromGameObjectRenderers(sabers);
            foreach (Material trailMaterial in sabers.GetComponentsInChildren<CustomTrail>().Select(t => t.TrailMaterial))
            {
                if (!materials.Contains(trailMaterial))
                {
                    materials.Add(trailMaterial);
                }
            }
            await ShaderRepair.FixShadersOnMaterialsAsync(materials);

            return sabers;
        }
    }
}
