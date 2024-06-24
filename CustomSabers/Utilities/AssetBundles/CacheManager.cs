using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Utilities.AssetBundles;

internal class CacheManager : IInitializable
{
    private readonly CSLConfig config;
    private readonly CustomSabersLoader customSabersLoader;

    private readonly string sabersPath;
    private readonly string cachePath;
    private readonly string deletedSabersPath;

    public Task CacheInitialization { get; }

    public CacheManager(PluginDirs pluginDirs, CSLConfig config, CustomSabersLoader customSabersLoader)
    {
        this.config = config;
        this.customSabersLoader = customSabersLoader;

        sabersPath = pluginDirs.CustomSabers.FullName;
        cachePath = pluginDirs.Cache.FullName;
        deletedSabersPath = pluginDirs.DeletedSabers.FullName;

        CacheInitialization = LoadAsync();
    }

    public int SelectedSaberIndex { get; set; } = 0; // used by UI to get position on the saber list 

    public List<CustomSaberMetadata> SabersMetadata { get; private set; } = [];

    private readonly string[] metaExt = [FileExts.Metadata];
    private readonly string[] saberExt = [FileExts.Saber, FileExts.Whacker];

    public void Initialize()
    {
        if (config.PluginVer != Plugin.Version.ToString())
        {
            Logger.Debug("Mod version has changed! Clearing cache");
            ClearCache();
            config.PluginVer = Plugin.Version.ToString();
        }

        Logger.Debug("Starting the CustomSabersAssetLoader");
        Task.Run(LoadAsync);
    }

    private async Task LoadAsync()
    {
        var fileMetadata = GetCachedMetadata();

        var sabersToLoad = GetSaberFiles(true).Where(f => !fileMetadata.ContainsKey(f));
        var loadedSaberData = await customSabersLoader.LoadCustomSabersAsync(sabersToLoad);

        UpdateCache(fileMetadata, loadedSaberData);

        if (config.CurrentlySelectedSaber == null)
        {
            SelectedSaberIndex = 0;
            return;
        }

        for (var i = 0; i < SabersMetadata.Count(); i++)
        {
            if (SabersMetadata[i].RelativePath == config.CurrentlySelectedSaber)
            {
                SelectedSaberIndex = i;
                break;
            }
        }
    }

    public async Task ReloadAsync()
    {
        Logger.Debug("Reloading the CustomSaberAssetLoader");
        await LoadAsync();
    }

    private IEnumerable<string> GetSaberFiles(bool returnShortPath) =>
        FileUtils.GetFilePaths(sabersPath, saberExt, searchOption: SearchOption.AllDirectories, returnShortPath);

    private IEnumerable<string> GetMetadataFiles(bool returnShortPath) =>
        FileUtils.GetFilePaths(cachePath, metaExt, searchOption: SearchOption.TopDirectoryOnly, returnShortPath);

    private Dictionary<string, CustomSaberMetadata> GetCachedMetadata()
    {
        Dictionary<string, CustomSaberMetadata> fileMetadata = [];

        foreach (var metaFile in GetMetadataFiles(false))
        {
            var metadata = JsonConvert.DeserializeObject<CustomSaberMetadata>(File.ReadAllText(metaFile));
            if (metadata.RelativePath == null) continue;

            if (File.Exists(Path.Combine(sabersPath, metadata.RelativePath)))
            {
                fileMetadata.Add(metadata.RelativePath, metadata);
            }
        }

        return fileMetadata;
    }

    private void UpdateCache(Dictionary<string, CustomSaberMetadata> fileMetadata, IEnumerable<CustomSaberData> loadedSaberData)
    {
        foreach (var saber in loadedSaberData)
        {
            var metadata = new CustomSaberMetadata
            {
                SaberName = saber.Descriptor.SaberName,
                AuthorName = saber.Descriptor.AuthorName,
                RelativePath = saber.FilePath,
                MissingShaders = saber.MissingShaders,
                CoverImage = saber.Descriptor.CoverImage?.texture.DuplicateTexture().Downscale(128, 128).EncodeToPNG(),
            };

            var metaFileName = Path.GetFileNameWithoutExtension(saber.FilePath) + FileExts.Metadata;

            // Cache data for each loaded saber
            var metaFilePath = Path.Combine(cachePath, metaFileName);

            try
            {
                if (File.Exists(metaFilePath))
                {
                    File.Delete(metaFilePath);
                }
                File.WriteAllText(metaFilePath, JsonConvert.SerializeObject(metadata));
            }
            catch (Exception ex)
            {
                Logger.Error($"Problem encountered when trying to cache a saber's metadata\n{ex.Message}\n{ex}");
                continue;
            }

            fileMetadata.Add(metaFilePath, metadata);
            saber.Destroy();
        }

        SabersMetadata.Clear();
        SabersMetadata.Add(new CustomSaberMetadata() { SaberName = "Default", AuthorName = "Beat Games" });
        SabersMetadata.AddRange(fileMetadata.Values);
    }

    private void ClearCache()
    {
        foreach (var metaFilePath in GetMetadataFiles(false))
        {
            var fileName = Path.GetFileName(metaFilePath);
            var destinationPath = Path.Combine(deletedSabersPath, fileName);

            try
            {
                if (File.Exists(destinationPath)) File.Delete(destinationPath);
                File.Move(metaFilePath, destinationPath);
            }
            catch (Exception ex)
            {
                Logger.Error($"Problem encountered when trying to clear {fileName} from cache\n{ex}");
            }
        }
    }
}
