using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using AssetBundleLoadingTools.Utilities;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using UnityEngine;
using System.Diagnostics;
using CustomSaber;
using Zenject;

namespace CustomSabersLite.Utilities.AssetBundles
{
    /// <summary>
    /// Handles loading and storing .saber files - is planned to be split up
    /// </summary>
    internal class CSLAssetLoader : IInitializable, IDisposable
    {
        private readonly PluginDirs pluginDirs;
        private readonly CSLConfig config;

        public CSLAssetLoader(PluginDirs pluginDirs, CSLConfig config)
        {
            this.pluginDirs = pluginDirs;
            this.config = config;
        }

        public bool IsLoaded {  get; private set; }

        public int SelectedSaberIndex { get; internal set; } = 0;

        public CustomSaberData SelectedSaber { get; set; }

        public List<CustomSaberMetadata> SabersMetadata { get; private set; } = new List<CustomSaberMetadata>();

        public IEnumerable<string> CustomSaberFiles { get; private set; } = Enumerable.Empty<string>();
        
        public IEnumerable<string> SaberMetadataFiles { get; private set; } = Enumerable.Empty<string>();

        private List<string> sabersToLoad = new List<string>();

        private IList<CustomSaberData> loadedSaberData = new List<CustomSaberData>();

        private string saberAssetPath;
        private string cachePath;
        private string deletedSabersPath;

        public async void Initialize()
        {
            saberAssetPath = pluginDirs.CustomSabers.FullName;
            cachePath = pluginDirs.Cache.FullName;
            deletedSabersPath = pluginDirs.DeletedSabers.FullName;

            if (config.PluginVer != Plugin.Version)
            {
                Logger.Debug("Mod version has changed! Clearing cache");
                ClearCache();
                config.PluginVer = Plugin.Version;
            }

            Logger.Debug("Starting the CustomSabersAssetLoader");
            Load();
        }

        public void Dispose() => Clear();

        internal async Task LoadAsync()
        {
            Dictionary<string, CustomSaberMetadata> fileMetadata = GetMetadataFromFiles();

            if (sabersToLoad.Count > 0)
            {
                loadedSaberData = await LoadCustomSabersAsync(sabersToLoad);
            }

            IsLoaded = true;

            UpdateCache(fileMetadata, loadedSaberData);

            if (config.CurrentlySelectedSaber != null)
            {
                Logger.Debug($"Currently selected saber: {config.CurrentlySelectedSaber}");

                SelectedSaber = await LoadSaberFromAssetAsync(config.CurrentlySelectedSaber);

                for (int i = 0; i < SabersMetadata.Count(); i++)
                {
                    if (SabersMetadata[i].RelativePath == config.CurrentlySelectedSaber)
                    {
                        SelectedSaberIndex = i;
                        break;
                    }
                }
            }
        }

        internal void Load()
        {
            Dictionary<string, CustomSaberMetadata> fileMetadata = GetMetadataFromFiles();

            if (sabersToLoad.Count > 0)
            {
                loadedSaberData = LoadCustomSabers(sabersToLoad);
            }

            IsLoaded = true;

            UpdateCache(fileMetadata, loadedSaberData);

            if (config.CurrentlySelectedSaber != null)
            {
                Logger.Debug($"Currently selected saber: {config.CurrentlySelectedSaber}");

                SelectedSaber = LoadSaberWithRepair(config.CurrentlySelectedSaber);

                for (int i = 0; i < SabersMetadata.Count(); i++)
                {
                    if (SabersMetadata[i].RelativePath == config.CurrentlySelectedSaber)
                    {
                        SelectedSaberIndex = i;
                        break;
                    }
                }
            }
        }

        internal async Task ReloadAsync()
        {
            Logger.Debug("Reloading the CustomSaberAssetLoader");
            Clear();
            await LoadAsync();
        }

        internal void Clear()
        {
            SelectedSaber?.Destroy();
            SelectedSaber = null;

            sabersToLoad.Clear();
            sabersToLoad = new List<string>();

            loadedSaberData.Clear();
            loadedSaberData = new List<CustomSaberData>();

            SabersMetadata.Clear();
            SabersMetadata = new List<CustomSaberMetadata>();

            CustomSaberFiles = Enumerable.Empty<string>();

            SaberMetadataFiles = Enumerable.Empty<string>();
        }

        private IEnumerable<string> GetSaberFiles(bool returnShortPath)
        {
            IEnumerable<string> saberExt = new List<string> { ".saber" };
            IEnumerable<string> saberFiles =
                CSLUtils.GetFilePaths(
                    saberAssetPath,
                    saberExt,
                    searchOption: SearchOption.AllDirectories,
                    returnShortPath
                );
            Logger.Info($"{saberFiles.Count()} external sabers found");
            return saberFiles;
        }

        private IEnumerable<string> GetMetadataFiles(bool returnShortPath)
        {
            IEnumerable<string> metaExt = new List<string> { ".meta" };
            IEnumerable<string> metadataFiles = 
                CSLUtils.GetFilePaths(
                    cachePath, 
                    metaExt, 
                    searchOption: SearchOption.TopDirectoryOnly, 
                    returnShortPath
                );
            Logger.Info($"{metadataFiles.Count()} metadata files found");
            return metadataFiles;
        }

        private Dictionary<string, CustomSaberMetadata> GetMetadataFromFiles()
        {
            CustomSaberFiles = GetSaberFiles(true);

            SaberMetadataFiles = GetMetadataFiles(false);

            Dictionary<string, CustomSaberMetadata> fileMetadata = new Dictionary<string, CustomSaberMetadata>();

            Stopwatch sw = Stopwatch.StartNew();
            foreach (string filePath in SaberMetadataFiles)
            {
                string json = File.ReadAllText(filePath);
                CustomSaberMetadata metadata = JsonConvert.DeserializeObject<CustomSaberMetadata>(json);
                string saberFileName = metadata.RelativePath;
                string saberPath = Path.Combine(saberAssetPath, saberFileName);
                if (File.Exists(saberPath))
                {
                    fileMetadata.Add(saberFileName, metadata);
                }
            }
            sw.Stop();
            Logger.Debug($"{SaberMetadataFiles.Count()} metadata files read in {sw.ElapsedMilliseconds}ms");

            foreach (string saberFile in CustomSaberFiles)
            {
                if (!fileMetadata.ContainsKey(saberFile))
                {
                    sabersToLoad.Add(saberFile);
                }
            }

            return fileMetadata;
        }

        private void UpdateCache(Dictionary<string, CustomSaberMetadata> fileMetadata, IList<CustomSaberData> loadedSaberData)
        {
            CustomSaberMetadata defaultSabers = new CustomSaberMetadata("Default", "Beat Games", "Default");
            SabersMetadata.Add(defaultSabers);

            foreach (CustomSaberData saber in loadedSaberData)
            {
                if (saber.FilePath == "Default") continue;

                byte[] coverImage = null;
                if (saber.Descriptor.CoverImage != null)
                {
                    try
                    {
                        coverImage = ImageLoading.DuplicateTexture(saber.Descriptor.CoverImage.texture).EncodeToPNG();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error("Problem encountered when reading stored image");
                        Logger.Error(ex.Message);
                    }
                }

                CustomSaberMetadata metadata = new CustomSaberMetadata(
                    saber.Descriptor.SaberName,
                    saber.Descriptor.AuthorName,
                    saber.FilePath,
                    saber.MissingShaders,
                    coverImage);

                string metaFileName = Path.GetFileNameWithoutExtension(saber.FilePath) + ".meta";

                // Cache data for each loaded saber
                string metaFilePath = Path.Combine(cachePath, metaFileName);

                if (!File.Exists(metaFilePath))
                {
                    string json = JsonConvert.SerializeObject(metadata);
                    File.WriteAllText(metaFilePath, json);
                    fileMetadata.Add(metaFilePath, metadata);
                }

                saber.Destroy();
            }

            SabersMetadata.AddRange(fileMetadata.Values);
        }

        internal void ClearCache()
        {
            SaberMetadataFiles = GetMetadataFiles(false);

            foreach (string metaFilePath in SaberMetadataFiles)
            {
                string fileName = Path.GetFileName(metaFilePath);
                string destinationPath = Path.Combine(deletedSabersPath, fileName);

                if (File.Exists(destinationPath)) File.Delete(destinationPath);

                File.Move(metaFilePath, destinationPath);
            }
        }

        private async Task<IList<CustomSaberData>> LoadCustomSabersAsync(IEnumerable<string> customSaberFiles)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            IList<CustomSaberData> customSabers = new List<CustomSaberData>();

            foreach (string file in customSaberFiles)
            {
                Logger.Debug($"No cache for {file}\nLoading saber from asset: {file}");

                customSabers.Add(await Task.Run(() => LoadSaberFromAssetAsync(file)));
                // customSabers.Add(await LoadSaberFromAssetAsync(file));
            }

            stopwatch.Stop();
            Logger.Info($"{customSabers.Count} total sabers loaded in {stopwatch.ElapsedMilliseconds}ms");

            return customSabers;
        }

        private IList<CustomSaberData> LoadCustomSabers(IEnumerable<string> customSaberFiles)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            IList<CustomSaberData> customSabers = new List<CustomSaberData>();

            foreach (string file in customSaberFiles)
            {
                Logger.Debug($"No cache for {file}\nLoading saber from asset: {file}");

                customSabers.Add(LoadSaberWithRepair(file));
            }

            stopwatch.Stop();
            Logger.Info($"{customSabers.Count} total sabers loaded in {stopwatch.ElapsedMilliseconds}ms");

            return customSabers;
        }

        private static CustomSaberData FixSaberShaders(CustomSaberData saber)
        {
            Logger.Debug($"Repairing shaders for {saber.FilePath}");
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
                    Logger.Warn($"Missing shader replacement data:");
                    foreach (var shaderName in replacementInfo.MissingShaderNames)
                    {
                        Logger.Warn($"\t- {shaderName}");
                    }
                    saber.MissingShaders = true;
                }
                else { saber.MissingShaders = false; Logger.Debug("All shaders replaced!"); }
            }
            catch (Exception ex)
            {
                Logger.Warn($"Problem encountered when repairing shaders for {saber.FilePath}");
                Logger.Error(ex.Message);
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
                    Logger.Warn($"Missing shader replacement data for {saber.FilePath}:");
                    foreach (var shaderName in replacementInfo.MissingShaderNames)
                    {
                        Logger.Warn($"\t- {shaderName}");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"Problem encountered when repairing shaders for {saber.FilePath}");
                Logger.Error(ex.Message);
            }
            return saber;
        }

        private async Task<CustomSaberData> LoadSaberFromAssetAsync(string fileName)
        {
            CustomSaberData newSaberData;
            GameObject sabers = null;
            SaberDescriptor descriptor;
            string filePath = Path.Combine(saberAssetPath, fileName);

            if (fileName != "Default")
            {
                try
                {
                    if (!File.Exists(filePath))
                    {
                        Logger.Warn($"{fileName} no longer exists, switching to default sabers");
                        config.CurrentlySelectedSaber = "Default";
                        return new CustomSaberData("Default");
                    }

                    AssetBundle bundle = await AssetBundleExtensions.LoadFromFileAsync(filePath);
                    sabers = await AssetBundleExtensions.LoadAssetAsync<GameObject>(bundle, "_CustomSaber");
                    bundle.UnloadAsync(false);

                    descriptor = sabers.GetComponent<SaberDescriptor>();
                    descriptor.CoverImage = descriptor.CoverImage ?? null;
                }
                catch
                {
                    Logger.Warn($"Problem encountered when getting the AssetBundle for {fileName}");

                    descriptor = new SaberDescriptor
                    {
                        SaberName = "Invalid Saber",
                        AuthorName = fileName
                    };

                    fileName = "Default";
                }

                newSaberData = new CustomSaberData(fileName)
                {
                    SabersObject = sabers,
                    Descriptor = descriptor
                };
            }
            else
            {
                newSaberData = new CustomSaberData("Default");
            }
            return newSaberData;
        }

        public CustomSaberData LoadSaberFromAsset(string fileName)
        {
            CustomSaberData newSaberData;
            GameObject sabers = null;
            SaberDescriptor descriptor;
            string filePath = Path.Combine(saberAssetPath, fileName);

            if (fileName != "Default")
            {
                try
                {
                    if (!File.Exists(filePath))
                    {
                        Logger.Warn($"{fileName} no longer exists, switching to default sabers");
                        config.CurrentlySelectedSaber = "Default";
                        return new CustomSaberData("Default");
                    }

                    AssetBundle bundle = AssetBundle.LoadFromFile(filePath);
                    sabers = bundle.LoadAsset<GameObject>("_CustomSaber");
                    bundle.Unload(false);

                    descriptor = sabers.GetComponent<SaberDescriptor>();
                    descriptor.CoverImage = descriptor.CoverImage ?? null;
                }
                catch
                {
                    Logger.Warn($"Problem encountered when getting the AssetBundle for {fileName}");

                    descriptor = new SaberDescriptor
                    {
                        SaberName = "Invalid Saber",
                        AuthorName = fileName
                    };

                    fileName = "Default";
                }

                newSaberData = new CustomSaberData(fileName)
                {
                    SabersObject = sabers,
                    Descriptor = descriptor
                };
            }
            else
            {
                newSaberData = new CustomSaberData("Default");
            }
            return newSaberData;
        }

        public CustomSaberData LoadSaberWithRepair(string fileName)
        {
            CustomSaberData saber = LoadSaberFromAsset(fileName);

            if (saber.FilePath != "Default") saber = FixSaberShaders(saber);

            return saber;
        }
    }
}
