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
using System.Text.RegularExpressions;

namespace CustomSaber.Utilities
{
    internal class CustomSaberAssetLoader
    {
        public static bool IsLoaded {  get; private set; }

        public static int SelectedSaberIndex { get; internal set; } = 0;

        public static CustomSaberData SelectedSaber { get; set; }

        public static List<CustomSaberMetadata> SabersMetadata { get; private set; } = new List<CustomSaberMetadata>();

        public static IEnumerable<string> CustomSaberFiles { get; private set; } = Enumerable.Empty<string>();
        
        public static IEnumerable<string> SaberMetadataFiles { get; private set; } = Enumerable.Empty<string>();

        private static List<string> sabersToLoad = new List<string>();

        private static IList<CustomSaberData> loadedSaberData = new List<CustomSaberData>();

        private static readonly string saberAssetPath = PluginDirs.CustomSabers.FullName;

        private static readonly string cachePath = PluginDirs.Cache.FullName;

        private static readonly Regex invalidPathChars = new Regex(new string(Path.GetInvalidFileNameChars()));

        internal static async Task Init()
        {
            if (CustomSaberConfig.Instance.PluginVer != Plugin.Version)
            {
                Plugin.Log.Info("Mod version has changed! Clearing cache");
                ClearCache();
                CustomSaberConfig.Instance.PluginVer = Plugin.Version;
            }

            Plugin.Log.Debug("Starting the CustomSabersAssetLoader");
            Load();
        }

        internal static async Task LoadAsync()
        {
            Dictionary<string, CustomSaberMetadata> fileMetadata = GetMetadataFromFiles();

            if (sabersToLoad.Count > 0)
            {
                loadedSaberData = await LoadCustomSabersAsync(sabersToLoad);
            }

            IsLoaded = true;

            UpdateCache(fileMetadata);

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
            Dictionary<string, CustomSaberMetadata> fileMetadata = GetMetadataFromFiles();

            if (sabersToLoad.Count > 0)
            {
                loadedSaberData = LoadCustomSabers(sabersToLoad);
            }

            IsLoaded = true;

            UpdateCache(fileMetadata);

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

        private static IEnumerable<string> GetSaberFiles(bool returnShortPath = false)
        {
            IEnumerable<string> saberExt = new List<string> { "*.saber" };
            IEnumerable<string> saberFiles = CustomSaberUtils.GetFileNames(saberAssetPath, saberExt, SearchOption.AllDirectories, returnShortPath);
            Plugin.Log.Info($"{saberFiles.Count()} external sabers found");
            return saberFiles;
        }

        private static IEnumerable<string> GetMetadataFiles(bool returnShortPath = false)
        {
            IEnumerable<string> metaExt = new List<string> { "*.meta" };
            IEnumerable<string> saberMetadataFiles = CustomSaberUtils.GetFileNames(cachePath, metaExt, SearchOption.AllDirectories, returnShortPath);
            Plugin.Log.Info($"{saberMetadataFiles.Count()} metadata files found");
            return saberMetadataFiles;
        }

        private static Dictionary<string, CustomSaberMetadata> GetMetadataFromFiles()
        {
            CustomSaberFiles = GetSaberFiles(true);

            SaberMetadataFiles = GetMetadataFiles(false);

            Dictionary<string, CustomSaberMetadata> fileMetadata = new Dictionary<string, CustomSaberMetadata>();

            Stopwatch sw = Stopwatch.StartNew();
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
            sw.Stop();
            Plugin.Log.Debug($"{SaberMetadataFiles.Count()} metadata files read in {sw.ElapsedMilliseconds}ms");

            foreach (string saberFile in CustomSaberFiles)
            {
                if (!fileMetadata.ContainsKey(saberFile))
                {
                    sabersToLoad.Add(saberFile);
                }
            }

            return fileMetadata;
        }

        private static void UpdateCache(Dictionary<string, CustomSaberMetadata> fileMetadata)
        {
            CustomSaberMetadata defaultSabers = new CustomSaberMetadata("Default", "Beat Games");
            SabersMetadata.Add(defaultSabers);

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

                bool nameContainsInvalidChars = false;
                if (invalidPathChars.IsMatch(saber.Descriptor.SaberName))
                {
                    saber.Descriptor.SaberName = string.Concat(saber.Descriptor.SaberName.Split(Path.GetInvalidFileNameChars()));
                    Plugin.Log.Warn($"{saber.Descriptor.SaberName}: descriptor contains invalid file name characters! If you are the saber creator please consider changing this.");
                    nameContainsInvalidChars = true;
                }

                CustomSaberMetadata metadata = new CustomSaberMetadata(saber.FileName, saber.Descriptor.AuthorName, saber.MissingShaders, nameContainsInvalidChars, coverImage);

                // Cache data for each loaded saber
                string metaFilePath = Path.Combine(cachePath, saber.Descriptor.SaberName + ".meta");

                if (!File.Exists(metaFilePath))
                {
                    string json = JsonConvert.SerializeObject(metadata);
                    File.WriteAllText(metaFilePath, json);
                    fileMetadata.Add(metaFilePath, metadata);
                }

                saber.Destroy();
            }

            foreach (CustomSaberMetadata customMetadata in fileMetadata.Values)
            {
                SabersMetadata.Add(customMetadata);
            }
        }

        internal static void ClearCache()
        {
            SaberMetadataFiles = GetMetadataFiles(false);

            foreach (string metaFilePath in SaberMetadataFiles)
            {
                string fileName = Path.GetFileName(metaFilePath);
                string destinationPath = Path.Combine(PluginDirs.DeletedSabers.FullName, fileName);

                if (File.Exists(destinationPath)) File.Delete(destinationPath);

                File.Move(metaFilePath, destinationPath);
            }
        }

        private static async Task<IList<CustomSaberData>> LoadCustomSabersAsync(IEnumerable<string> customSaberFiles)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            IList<CustomSaberData> customSabers = new List<CustomSaberData>();

            foreach (string file in customSaberFiles)
            {
                Plugin.Log.Debug($"No cache for {file}\nLoading saber from asset: {file}");

                customSabers.Add(await Task.Run(() => LoadSaberFromAssetAsync(file)));
                // customSabers.Add(await LoadSaberFromAssetAsync(file));
            }

            stopwatch.Stop();
            Plugin.Log.Info($"{customSabers.Count} total sabers loaded in {stopwatch.ElapsedMilliseconds}ms");

            return customSabers;
        }

        private static IList<CustomSaberData> LoadCustomSabers(IEnumerable<string> customSaberFiles)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            IList<CustomSaberData> customSabers = new List<CustomSaberData>();

            foreach (string file in customSaberFiles)
            {
                Plugin.Log.Debug($"No cache for {file}\nLoading saber from asset: {file}");

                customSabers.Add(LoadSaberWithRepair(file));
            }

            stopwatch.Stop();
            Plugin.Log.Info($"{customSabers.Count} total sabers loaded in {stopwatch.ElapsedMilliseconds}ms");

            return customSabers;
        }

        private static CustomSaberData FixSaberShaders(CustomSaberData saber)
        {
            Plugin.Log.Debug($"Repairing shaders for {saber.FileName}");
            try
            {
                List<Material> materials = ShaderRepair.GetMaterialsFromGameObjectRenderers(saber.SabersObject);

                // Manually add CustomTrails to materials list
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

                // Manually add CustomTrails to materials list
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

            if (saber.FileName != "DefaultSabers") saber = FixSaberShaders(saber);

            return saber;
        }
    }
}
