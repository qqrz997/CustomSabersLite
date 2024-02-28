using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using AssetBundleLoadingTools.Utilities;
using CustomSaber.Configuration;
using CustomSaber.Data;
using UnityEngine;
using System.Diagnostics;

namespace CustomSaber.Utilities
{
    public class CustomSaberAssetLoader
    {
        public static bool IsLoaded {  get; private set; }

        public static int SelectedSaberIndex { get; internal set; } = 0;

        public static CustomSaberData SelectedSaber { get; set; }

        public static List<CustomSaberMetadata> SabersMetadata { get; private set; } = new List<CustomSaberMetadata>();

        public static IEnumerable<string> CustomSaberFiles { get; private set; } = Enumerable.Empty<string>();
        
        public static IEnumerable<string> SaberMetadataFiles { get; private set; } = Enumerable.Empty<string>();

        private static List<string> sabersToLoad = new List<string>();

        private static IList<CustomSaberData> loadedSaberData = new List<CustomSaberData>();

        public static Action customSabersLoaded;

        private static readonly string saberAssetPath = PluginDirs.CustomSabers.FullName;

        private static readonly string cachePath = PluginDirs.Cache.FullName;

        internal static async Task LoadAsync()
        {
            Stopwatch timer;

            IEnumerable<string> saberExt = new List<string> { "*.saber" };

            IEnumerable<string> metaExt = new List<string> { "*.json" };

            CustomSaberFiles = CustomSaberUtils.GetFileNames(saberAssetPath, saberExt, SearchOption.AllDirectories, true);
            Plugin.Log.Info($"{CustomSaberFiles.Count()} external sabers found.");

            SaberMetadataFiles = CustomSaberUtils.GetFileNames(cachePath, metaExt, SearchOption.AllDirectories, false);
            Plugin.Log.Info($"{SaberMetadataFiles.Count()} metadata files found.");

            Dictionary<string, CustomSaberMetadata> fileMetadata = new Dictionary<string, CustomSaberMetadata>();

            foreach (string filePath in SaberMetadataFiles)
            {
                string json = File.ReadAllText(filePath);
                CustomSaberMetadata metadata = JsonConvert.DeserializeObject<CustomSaberMetadata>(json);
                string saberFileName = metadata.SaberFileName;
                string saberPath = Path.Combine(saberAssetPath, saberFileName);
                if (File.Exists(saberPath))
                {
                    fileMetadata.Add(saberFileName, metadata);
                }

            }

            foreach (string saberFile in CustomSaberFiles)
            {
                if (!fileMetadata.ContainsKey(saberFile))
                {
                    sabersToLoad.Add(saberFile);
                }
            }

            if (sabersToLoad.Count > 0)
            {
                timer = Stopwatch.StartNew();
                loadedSaberData = await LoadCustomSabersAsync(sabersToLoad);
                timer.Stop();
                Plugin.Log.Info($"{loadedSaberData.Count} total sabers loaded in {timer.ElapsedMilliseconds}ms");
            }

            IsLoaded = true;
            customSabersLoaded?.Invoke();

            foreach (CustomSaberData saber in loadedSaberData)
            {
                if (saber.FileName == "DefaultSabers") continue;

                byte[] coverImage = null;
                if (saber.Descriptor.CoverImage != null)
                {
                    coverImage = ImageLoading.DuplicateTexture(saber.Descriptor.CoverImage.texture).EncodeToPNG();
                }

                CustomSaberMetadata metadata = new CustomSaberMetadata(saber.FileName, saber.Descriptor.AuthorName, saber.MissingShaders, coverImage);

                //Cache data for each loaded saber
                string metaFilePath = Path.Combine(cachePath, saber.Descriptor.SaberName + ".json");
                string json = JsonConvert.SerializeObject(metadata);

                if (!File.Exists(metaFilePath))
                {
                    File.WriteAllText(metaFilePath, json);
                    fileMetadata.Add(metaFilePath, metadata);
                }

                saber.Destroy();
            }

            CustomSaberMetadata defaultSabers = new CustomSaberMetadata("Default", "Beat Games", false, null);
            SabersMetadata.Add(defaultSabers);
            foreach (CustomSaberMetadata customMetadata in fileMetadata.Values)
            {
                SabersMetadata.Add(customMetadata);
            }

            if (CustomSaberConfig.Instance.CurrentlySelectedSaber != null)
            {
                Plugin.Log.Debug($"Currently selected saber: {CustomSaberConfig.Instance.CurrentlySelectedSaber}");

                SelectedSaber = await LoadSaberFromAssetAsync(CustomSaberConfig.Instance.CurrentlySelectedSaber);

                for (int i = 0; i < SabersMetadata.Count(); i++)
                {
                    if (SabersMetadata[i].SaberFileName == CustomSaberConfig.Instance.CurrentlySelectedSaber)
                    {
                        SelectedSaberIndex = i;
                        break;
                    }
                }
            }
        }

        internal static void Load()
        {
            Stopwatch timer;

            IEnumerable<string> saberExt = new List<string> { "*.saber" };

            IEnumerable<string> metaExt = new List<string> { "*.json" };

            CustomSaberFiles = CustomSaberUtils.GetFileNames(saberAssetPath, saberExt, SearchOption.AllDirectories, true);
            Plugin.Log.Info($"{CustomSaberFiles.Count()} external sabers found.");

            SaberMetadataFiles = CustomSaberUtils.GetFileNames(cachePath, metaExt, SearchOption.AllDirectories, false);
            Plugin.Log.Info($"{SaberMetadataFiles.Count()} metadata files found.");

            Dictionary<string, CustomSaberMetadata> fileMetadata = new Dictionary<string, CustomSaberMetadata>();

            foreach (string filePath in SaberMetadataFiles)
            {
                string json = File.ReadAllText(filePath);
                CustomSaberMetadata metadata = JsonConvert.DeserializeObject<CustomSaberMetadata>(json);
                string saberFileName = metadata.SaberFileName;
                string saberPath = Path.Combine(saberAssetPath, saberFileName);
                if (File.Exists(saberPath))
                {
                    fileMetadata.Add(saberFileName, metadata);
                }
            }

            foreach (string saberFile in CustomSaberFiles)
            {
                if (!fileMetadata.ContainsKey(saberFile))
                {
                    sabersToLoad.Add(saberFile);
                }
            }

            if (sabersToLoad.Count > 0)
            {
                timer = Stopwatch.StartNew();
                loadedSaberData = LoadCustomSabers(sabersToLoad);
                timer.Stop();
                Plugin.Log.Info($"{loadedSaberData.Count} total sabers loaded in {timer.ElapsedMilliseconds}ms");
            }

            IsLoaded = true;
            customSabersLoaded?.Invoke();

            foreach (CustomSaberData saber in loadedSaberData)
            {
                if (saber.FileName == "DefaultSabers") continue;

                byte[] coverImage = null;
                if (saber.Descriptor.CoverImage != null)
                {
                    try
                    {
                        coverImage = ImageLoading.DuplicateTexture(saber.Descriptor.CoverImage.texture).EncodeToPNG();
                    }
                    catch (Exception ex)
                    {
                        Plugin.Log.Error("Problem encountered when reading stored image");
                        Plugin.Log.Error(ex);
                    }
                }

                try
                {
                    CustomSaberMetadata metadata = new CustomSaberMetadata(saber.FileName, saber.Descriptor.AuthorName, saber.MissingShaders, coverImage);

                    //Cache data for each loaded saber
                    string metaFilePath = Path.Combine(cachePath, saber.Descriptor.SaberName + ".json");
                    string json = JsonConvert.SerializeObject(metadata);

                    if (!File.Exists(metaFilePath))
                    {
                        File.WriteAllText(metaFilePath, json);
                        fileMetadata.Add(metaFilePath, metadata);
                    }
                }
                catch (Exception ex)
                {
                    Plugin.Log.Error($"Problem encountered when creating metadata for {saber.FileName}");
                    Plugin.Log.Error(ex);
                }

                saber.Destroy();
            }

            CustomSaberMetadata defaultSabers = new CustomSaberMetadata("Default", "Beat Games", false, null);
            SabersMetadata.Add(defaultSabers);
            foreach (CustomSaberMetadata customMetadata in fileMetadata.Values)
            {
                //only use metadata for sabers that currently exist
                if (File.Exists(Path.Combine(PluginDirs.CustomSabers.FullName, customMetadata.SaberFileName))) SabersMetadata.Add(customMetadata);
            }

            if (CustomSaberConfig.Instance.CurrentlySelectedSaber != null)
            {
                Plugin.Log.Debug($"Currently selected saber: {CustomSaberConfig.Instance.CurrentlySelectedSaber}");

                SelectedSaber = LoadSaberWithRepair(CustomSaberConfig.Instance.CurrentlySelectedSaber);

                for (int i = 0; i < SabersMetadata.Count(); i++)
                {
                    if (SabersMetadata[i].SaberFileName == CustomSaberConfig.Instance.CurrentlySelectedSaber)
                    {
                        SelectedSaberIndex = i;
                        break;
                    }
                }
            }
        }

        internal static async Task ReloadAsync()
        {
            Plugin.Log.Debug("Reloading the CustomSaberAssetLoader");
            Clear();
            await LoadAsync();
        }

        internal static void Clear()
        {
            for (int i = 0; i < loadedSaberData.Count; i++)
            {
                loadedSaberData[i].Destroy();
                loadedSaberData[i] = null;
            }

            if (SelectedSaber != null)
            {
                SelectedSaber.Destroy();
                SelectedSaber = null;
            }
            sabersToLoad.Clear();
            sabersToLoad = new List<string>();
            loadedSaberData.Clear();
            loadedSaberData = new List<CustomSaberData>();
            SabersMetadata.Clear();
            SabersMetadata = new List<CustomSaberMetadata>();
            CustomSaberFiles = Enumerable.Empty<string>();
            SaberMetadataFiles = Enumerable.Empty<string>();
        }

        private static async Task<IList<CustomSaberData>> LoadCustomSabersAsync(IEnumerable<string> customSaberFiles)
        {
            IList<CustomSaberData> customSabers = new List<CustomSaberData>();

            foreach (string file in customSaberFiles)
            {
                Plugin.Log.Debug($"No cache for {file}\nLoading saber from asset: {file}");

                customSabers.Add(await Task.Run(() => LoadSaberFromAssetAsync(file)));
                //customSabers.Add(await LoadSaberFromAssetAsync(file));
            }

            return customSabers;
        }

        private static IList<CustomSaberData> LoadCustomSabers(IEnumerable<string> customSaberFiles)
        {
            IList<CustomSaberData> customSabers = new List<CustomSaberData>();

            foreach (string file in customSaberFiles)
            {
                Plugin.Log.Debug($"No cache for {file}\nLoading saber from asset: {file}");

                customSabers.Add(LoadSaberWithRepair(file));
            }

            return customSabers;
        }

        private static CustomSaberData FixSaberShaders(CustomSaberData saber)
        {
            Plugin.Log.Debug($"Repairing shaders for {saber.FileName}");
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
                var replacementInfo = ShaderRepair.FixShadersOnMaterials(materials);
                if (!replacementInfo.AllShadersReplaced)
                {
                    Plugin.Log.Warn($"Missing shader replacement data:");
                    foreach (var shaderName in replacementInfo.MissingShaderNames)
                    {
                        Plugin.Log.Warn($"\t- {shaderName}");
                    }
                    saber.MissingShaders = true;
                }
                else { saber.MissingShaders = false; Plugin.Log.Debug("All shaders replaced!"); }
            }
            catch (Exception ex)
            {
                Plugin.Log.Warn($"Problem encountered when repairing shaders for {saber.FileName}");
                Plugin.Log.Error(ex);
            }
            return saber;
        }

        private static async Task<CustomSaberData> FixSaberShadersAsync(CustomSaberData saber)
        {
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
            return saber;
        }

        private static async Task<CustomSaberData> LoadSaberFromAssetAsync(string fileName)
        {
            CustomSaberData newSaberData;
            AssetBundle bundle = null;
            GameObject sabers = null;
            SaberDescriptor descriptor;
            string filePath = Path.Combine(saberAssetPath, fileName);

            if (fileName != "Default")
            {
                try
                {
                    if (!File.Exists(filePath))
                    {
                        Plugin.Log.Warn($"{fileName} no longer exists, switching to default sabers");
                        CustomSaberConfig.Instance.CurrentlySelectedSaber = "Default";
                        return new CustomSaberData("DefaultSabers");
                    }
                    bundle = await AssetBundleExtensions.LoadFromFileAsync(filePath);
                    sabers = await AssetBundleExtensions.LoadAssetAsync<GameObject>(bundle, "_CustomSaber");

                    descriptor = sabers.GetComponent<SaberDescriptor>();
                    descriptor.CoverImage = descriptor.CoverImage ?? null;
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

                newSaberData = new CustomSaberData(fileName)
                {
                    AssetBundle = bundle,
                    SabersObject = sabers,
                    Descriptor = descriptor
                };
            }
            else
            {
                newSaberData = new CustomSaberData("DefaultSabers");
            }

            return newSaberData;
        }

        public static CustomSaberData LoadSaberFromAsset(string fileName)
        {
            CustomSaberData newSaberData;
            AssetBundle bundle = null;
            GameObject sabers = null;
            SaberDescriptor descriptor;
            string filePath = Path.Combine(saberAssetPath, fileName);

            if (fileName != "Default")
            {
                try
                {
                    if (!File.Exists(filePath))
                    {
                        Plugin.Log.Warn($"{fileName} no longer exists, switching to default sabers");
                        CustomSaberConfig.Instance.CurrentlySelectedSaber = "Default";
                        return new CustomSaberData("DefaultSabers");
                    }
                    bundle = AssetBundle.LoadFromFile(filePath);
                    sabers = bundle.LoadAsset<GameObject>("_CustomSaber");

                    descriptor = sabers.GetComponent<SaberDescriptor>();
                    descriptor.CoverImage = descriptor.CoverImage ?? null;
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

                newSaberData = new CustomSaberData(fileName)
                {
                    AssetBundle = bundle,
                    SabersObject = sabers,
                    Descriptor = descriptor
                };
            }
            else
            {
                newSaberData = new CustomSaberData("DefaultSabers");
            }
            return newSaberData;
        }

        public static CustomSaberData LoadSaberWithRepair(string fileName)
        {
            CustomSaberData saber = LoadSaberFromAsset(fileName);

            saber = FixSaberShaders(saber);

            return saber;
        }

        //todo - saber deletion
        //public static int DeleteSelectedSaber(){}
    }
}
