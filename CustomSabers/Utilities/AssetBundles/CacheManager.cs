using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Utilities.AssetBundles;

internal class CacheManager(PluginDirs pluginDirs, CSLConfig config, CustomSabersLoader customSabersLoader) : IInitializable
{
    private readonly CSLConfig config = config;
    private readonly CustomSabersLoader customSabersLoader = customSabersLoader;

    private readonly string sabersPath = pluginDirs.CustomSabers.FullName;
    private readonly string cachePath = pluginDirs.Cache.FullName;
    private readonly string deletedSabersPath = pluginDirs.DeletedSabers.FullName;

    public event Action CacheInitializationSucceeded;
    public event Action CacheInitializationFailed;

    public bool Initialized { get; private set; }

    public int SelectedSaberIndex { get; set; } = 0; // used by UI to get position on the saber list 

    public List<CustomSaberMetadata> SabersMetadata { get; private set; } = [];

    private readonly string[] metaExt = [FileExts.Metadata];
    private readonly string[] saberExt = [FileExts.Saber, FileExts.Whacker];

    public async void Initialize()
    {
        if (config.PluginVer != Plugin.Version.ToString())
        {
            Logger.Debug("Mod version has changed! Clearing cache");
            ClearCache();
            config.PluginVer = Plugin.Version.ToString();
        }

        try
        {
            await LoadAsync();
            Logger.Info("Invoking");
            CacheInitializationSucceeded?.Invoke();
            Initialized = true;
        }
        catch (Exception ex)
        {
            CacheInitializationFailed?.Invoke();

            Logger.Warn($"{ex}");
        }
    }

    private async Task LoadAsync()
    {
        Logger.Info("Starting the CustomSabersAssetLoader");

        var fileMetadata = GetCachedMetadata();

        var sabersToLoad = GetSaberFiles(true).Where(f => !fileMetadata.ContainsKey(f));
        var loadedSaberData = await customSabersLoader.LoadCustomSabersAsync(sabersToLoad);

        await UpdateCache(fileMetadata, loadedSaberData);

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

    private async Task UpdateCache(Dictionary<string, CustomSaberMetadata> fileMetadata, IEnumerable<CustomSaberData> loadedSaberData)
    {
        foreach (var saberData in loadedSaberData)
        {
            var metadata = new CustomSaberMetadata
            {
                SaberName = saberData.Descriptor.SaberName,
                AuthorName = saberData.Descriptor.AuthorName,
                RelativePath = saberData.FilePath,
                MissingShaders = saberData.MissingShaders,
                CoverImage = saberData.Descriptor.CoverImage?.texture.DuplicateTexture().Downscale(128, 128).EncodeToPNG(),
            };

            var metaFileName = Path.GetFileNameWithoutExtension(saberData.FilePath) + FileExts.Metadata;
            var metaFilePath = Path.Combine(cachePath, metaFileName);
            
            if (!await WriteMetadataToFileAsync(metadata, metaFilePath))
                continue;

            fileMetadata.Add(metaFilePath, metadata);
            saberData.Destroy();
        }

        SabersMetadata.Clear();
        SabersMetadata.Add(new CustomSaberMetadata() { SaberName = "Default", AuthorName = "Beat Games" });
        SabersMetadata.AddRange(fileMetadata.Values);
    }

    private async Task<bool> WriteMetadataToFileAsync(CustomSaberMetadata metadata, string metaFilePath)
    {
        try
        {
            if (metaFilePath.ToLower().Contains("nut")) throw new Exception("THE SABER CAME");
            if (File.Exists(metaFilePath))
            {
                File.Delete(metaFilePath);
            }
            var encodedText = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(metadata));
            using var fileStream = new FileStream(metaFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            await fileStream.WriteAsync(encodedText, 0, encodedText.Length);
            return true;
        }
        catch (Exception ex)
        {
            Logger.Error($"Problem encountered when trying to cache a saber's metadata\n{ex.Message}\n{ex}");
            return false;
        }
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
