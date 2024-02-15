using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AssetBundleLoadingTools.Utilities;
using CustomSaber.Configuration;
using CustomSaber.Data;
using UnityEngine;

namespace CustomSaber.Utilities
{
    public class CustomSaberAssetLoader
    {
        public static bool IsLoaded {  get; private set; }

        public static int SelectedSaber { get; internal set; } = 0;

        public static IList<CustomSaberData> CustomSabers { get; private set; } = new List<CustomSaberData>();

        public static IEnumerable<string> CustomSaberFiles { get; private set; } = Enumerable.Empty<string>();

        public static Action customSabersLoaded;

        private static readonly Sprite nullCoverImage = CustomSaberUtils.GetNullCoverImage();

        internal static async Task Load()
        {
            var startTime = DateTime.Now;

            Directory.CreateDirectory(Plugin.CustomSaberAssetsPath);

            IEnumerable<string> saberExt = new List<string> { "*.saber" };

            Plugin.Log.Debug($"Searching directory for external sabers: {Plugin.CustomSaberAssetsPath}");

            CustomSaberFiles = CustomSaberUtils.GetFileNames(Plugin.CustomSaberAssetsPath, saberExt, SearchOption.AllDirectories, true);

            Plugin.Log.Info($"{CustomSaberFiles.Count()} external sabers found.");

            CustomSabers = await LoadCustomSabersAsync(CustomSaberFiles);

            Plugin.Log.Info($"{CustomSabers.Count} total sabers loaded in {Math.Floor((DateTime.Now - startTime).TotalMilliseconds)}ms");

            Plugin.Log.Debug($"Currently selected saber: {CustomSaberConfig.Instance.CurrentlySelectedSaber}");

            if (CustomSaberConfig.Instance.CurrentlySelectedSaber != null)
            {
                for (int i = 0; i < CustomSabers.Count; i++)
                {
                    if (CustomSabers[i].FileName == CustomSaberConfig.Instance.CurrentlySelectedSaber)
                    {
                        SelectedSaber = i;
                        break;
                    }
                }
            }

            IsLoaded = true;
            customSabersLoaded?.Invoke();
        }

        internal static async Task Reload()
        {
            Plugin.Log.Debug("Reloading the CustomSaberAssetLoader");
            Clear();
            await Load();
        }

        internal static void Clear()
        {
            int numberOfObjects = CustomSabers.Count;
            for (int i = 0; i < numberOfObjects; i++)
            {
                CustomSabers[i].Destroy();
                CustomSabers[i] = null;
            }

            SelectedSaber = 0;
            CustomSabers = new List<CustomSaberData>();
            CustomSaberFiles = Enumerable.Empty<string>();
        }

        private static async Task<IList<CustomSaberData>> LoadCustomSabersAsync(IEnumerable<string> customSaberFiles)
        {
            IList<CustomSaberData> customSabers = new List<CustomSaberData>
            {
                //Add the Default Sabers to the start of the list
                new CustomSaberData("DefaultSabers")
            };

            foreach (string file in customSaberFiles)
            {
                customSabers.Add(await Task.Run(() => LoadSaberFromAssetAsync(file)));
            }

            //Shader fix
            foreach (CustomSaberData saber in customSabers)
            {
                if (saber.FileName == "DefaultSabers")
                {
                    continue;
                }

                try
                {
                    List<Material> materials = ShaderRepair.GetMaterialsFromGameObjectRenderers(saber.SabersObject);

                    //Manually add CustomTrails to materials list
                    foreach (var customTrail in saber.SabersObject.GetComponentsInChildren<CustomTrail>(true))
                    {
                        if (!materials.Contains(customTrail.TrailMaterial))
                        {
                            materials.Add(customTrail.TrailMaterial);
                        }
                    }
                    var replacementInfo = await ShaderRepair.FixShadersOnMaterialsAsync(materials);
                    if (!replacementInfo.AllShadersReplaced)
                    {
                        Plugin.Log.Warn($"Missing shader replacement data for {saber.FileName}:");
                        foreach (var shaderName in replacementInfo.MissingShaderNames)
                        {
                            Plugin.Log.Warn($"\t- {shaderName}");
                        }
                    }
                } 
                catch (Exception ex) 
                {
                    Plugin.Log.Warn($"Problem encountered when repairing shaders for {saber.FileName}");
                    Plugin.Log.Error(ex);
                }
            }

            return customSabers;
        }

        private static async Task<CustomSaberData> LoadSaberFromAssetAsync(string fileName)
        {
            AssetBundle bundle = null;
            GameObject sabers = null;
            SaberDescriptor descriptor;

            Plugin.Log.Info($"Loading saber from {fileName}");
            try
            {
                bundle = await AssetBundleExtensions.LoadFromFileAsync(Path.Combine(Plugin.CustomSaberAssetsPath, fileName));
                sabers = await AssetBundleExtensions.LoadAssetAsync<GameObject>(bundle, "_CustomSaber");

                descriptor = sabers.GetComponent<SaberDescriptor>();
                descriptor.CoverImage = descriptor.CoverImage ?? nullCoverImage;
            }
            catch
            {
                Plugin.Log.Warn($"Problem encountered when getting the AssetBundle for {fileName}");

                descriptor = new SaberDescriptor
                {
                    SaberName = "Invalid Saber",
                    AuthorName = fileName
                };

                fileName = "DefaultSabers";
            }

            CustomSaberData newSaberData = new CustomSaberData(fileName)
            {
                AssetBundle = bundle,
                SabersObject = sabers,
                Descriptor = descriptor
            };

            return newSaberData;
        }
        
        //public static int DeleteSelectedSaber(){}
    }
}
