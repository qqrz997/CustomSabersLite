using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using UnityEngine;
using System.Diagnostics;
using Zenject;

namespace CustomSabersLite.Utilities.AssetBundles
{
    internal class CSLAssetLoader : IInitializable, IDisposable
    {
        private readonly CSLConfig config;
        private readonly ICustomSaberLoader customSaberLoader;
        private readonly IWhackerLoader whackerLoader;

        private readonly string sabersPath;
        private readonly string cachePath;
        private readonly string deletedSabersPath;

        public CSLAssetLoader(PluginDirs pluginDirs, CSLConfig config, ICustomSaberLoader customSaberLoader, IWhackerLoader whackerLoader)
        {
            this.config = config;
            this.customSaberLoader = customSaberLoader;
            this.whackerLoader = whackerLoader;

            sabersPath = pluginDirs.CustomSabers.FullName;
            cachePath = pluginDirs.Cache.FullName;
            deletedSabersPath = pluginDirs.DeletedSabers.FullName;
        }

        public int SelectedSaberIndex { get; internal set; } = 0; // used by UI to get position on the saber list 

        public List<CustomSaberMetadata> SabersMetadata { get; private set; } = new List<CustomSaberMetadata>();

        private readonly List<string> metaExt = new List<string> { FileExts.Metadata };
        private readonly List<string> saberExt = new List<string> { FileExts.Saber, FileExts.Whacker };

        public async void Initialize()
        {
            ClearCache();

            if (config.PluginVer != Plugin.Version)
            {
                Logger.Debug("Mod version has changed! Clearing cache");
                ClearCache();
                config.PluginVer = Plugin.Version;
            }

            Logger.Debug("Starting the CustomSabersAssetLoader");
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            Dictionary<string, CustomSaberMetadata> fileMetadata = GetCachedMetadata();

            List<string> sabersToLoad = new List<string>();
            foreach (string saberFile in GetSaberFiles(true))
            {
                if (!fileMetadata.ContainsKey(saberFile))
                {
                    sabersToLoad.Add(saberFile);
                }
            }

            List<CustomSaberData> loadedSaberData = await LoadCustomSabersAsync(sabersToLoad);

            UpdateCache(fileMetadata, loadedSaberData);

            if (config.CurrentlySelectedSaber != null)
            {
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

        public async Task ReloadAsync()
        {
            Logger.Debug("Reloading the CustomSaberAssetLoader");
            SabersMetadata.Clear();
            await LoadAsync();
        }

        private List<string> GetSaberFiles(bool returnShortPath) =>
            FileUtils.GetFilePaths(sabersPath, saberExt, searchOption: SearchOption.AllDirectories, returnShortPath);

        private List<string> GetMetadataFiles(bool returnShortPath) =>
            FileUtils.GetFilePaths(cachePath, metaExt, searchOption: SearchOption.TopDirectoryOnly, returnShortPath);

        private Dictionary<string, CustomSaberMetadata> GetCachedMetadata()
        {
            List<string> saberMetadataFiles = GetMetadataFiles(false);

            Dictionary<string, CustomSaberMetadata> fileMetadata = new Dictionary<string, CustomSaberMetadata>();

            foreach (string filePath in saberMetadataFiles)
            {
                string json = File.ReadAllText(filePath);
                CustomSaberMetadata metadata = JsonConvert.DeserializeObject<CustomSaberMetadata>(json);
                string saberFileName = metadata.RelativePath;
                string saberPath = Path.Combine(sabersPath, saberFileName);
                if (File.Exists(saberPath))
                {
                    fileMetadata.Add(saberFileName, metadata);
                }
            }

            return fileMetadata;
        }

        private void UpdateCache(Dictionary<string, CustomSaberMetadata> fileMetadata, List<CustomSaberData> loadedSaberData)
        {
            foreach (CustomSaberData saber in loadedSaberData)
            {
                CustomSaberMetadata metadata = new CustomSaberMetadata
                {
                    SaberName = saber.Descriptor.SaberName,
                    AuthorName = saber.Descriptor.AuthorName,
                    RelativePath = saber.FilePath,
                    MissingShaders = saber.MissingShaders,
                    CoverImage = saber.Descriptor.CoverImage == null ? null : ImageUtils.DuplicateTexture(saber.Descriptor.CoverImage.texture).EncodeToPNG(),
                };

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

            SabersMetadata.Add(new CustomSaberMetadata() { SaberName = "Default", AuthorName = "Beat Games" });
            SabersMetadata.AddRange(fileMetadata.Values);
        }

        private void ClearCache()
        {
            foreach (string metaFilePath in GetMetadataFiles(false))
            {
                string fileName = Path.GetFileName(metaFilePath);
                string destinationPath = Path.Combine(deletedSabersPath, fileName);

                if (File.Exists(destinationPath)) File.Delete(destinationPath);

                File.Move(metaFilePath, destinationPath);
            }
        }

        private async Task<List<CustomSaberData>> LoadCustomSabersAsync(IEnumerable<string> customSaberFiles)
        {
            List<CustomSaberData> customSabers = new List<CustomSaberData>();

            foreach (string file in customSaberFiles)
            {
                switch (Path.GetExtension(file))
                {
                    case ".saber":
                        customSabers.Add(await customSaberLoader.LoadCustomSaberAsync(file)); break;

                    case ".whacker":
                        customSabers.Add(await whackerLoader.LoadWhackerAsync(file)); break;
                }
            }

            return customSabers;
        }

        public void Dispose() => SabersMetadata.Clear();
    }
}
