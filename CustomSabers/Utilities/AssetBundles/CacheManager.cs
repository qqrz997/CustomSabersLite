using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

    // please please tell me if this is stupid
    public event Action<int> LoadingProgressChanged;
    public event Action LoadingComplete;
    
    public bool InitializationFinished { get; private set; }

    public bool InitializationFailed { get; private set; }

    public int SelectedSaberIndex { get; set; } = 0; // used by UI to get position on the saber list 

    public List<CustomSaberMetadata> SabersMetadata { get; private set; } = [];

    public async void Initialize()
    {
        if (config.PluginVer != Plugin.Version.ToString())
        {
            ClearCache();
            config.PluginVer = Plugin.Version.ToString();
        }

        await ReloadAsync();
    }

    public async Task ReloadAsync()
    {
        try
        {
            await InternalReloadAsync();
        }
        catch (Exception ex)
        {
            InitializationFailed = true;
            Logger.Critical($"Problem encountered during cache initialization - the mod will not activate\n{ex}");
        }
        finally
        {
            InitializationFinished = true;
            LoadingComplete?.Invoke();
        }
    }

    private async Task InternalReloadAsync()
    {
        Logger.Debug("Initializing caching step");
        Logger.Debug(config.CurrentlySelectedSaber != null 
            ? $"Selected saber is: {config.CurrentlySelectedSaber}" 
            : "No custom saber selected");

        var metaFilePaths = GetMetadataFiles(false);
        Logger.Debug($"Found {metaFilePaths.Length} meta files in cache");
        var existingFileMetadata = metaFilePaths
            .Select(File.ReadAllText)
            .Select(JsonConvert.DeserializeObject<CustomSaberMetadata>)
            .Where(metadata => metadata.RelativePath != null && File.Exists(Path.Combine(sabersPath, metadata.RelativePath)))
            .ToDictionary(metadata => metadata.RelativePath);

        var cachedMetadata = await UpdateCacheAsync(existingFileMetadata);
        Logger.Info($"Obtained metadata for {cachedMetadata.Count} sabers");

        var tmpRegex = new Regex(@"<[^>]*>");
        var sortedMetadata = cachedMetadata.OrderBy(v => tmpRegex.Replace(v.SaberName, string.Empty));

        SabersMetadata.Clear();
        SabersMetadata.Add(new CustomSaberMetadata() { SaberName = "Default", AuthorName = "Beat Games" });
        SabersMetadata.AddRange(sortedMetadata);

        var index = SabersMetadata.FindIndex(metadata => metadata.RelativePath == config.CurrentlySelectedSaber);
        SelectedSaberIndex = index < 0 ? 0 : index;
    }

    private async Task<List<CustomSaberMetadata>> UpdateCacheAsync(Dictionary<string, CustomSaberMetadata> existingFileMetadata)
    {
        var cachedMetadata = new List<CustomSaberMetadata>();
        var prevPercentProgress = 0;
        LoadingProgressChanged?.Invoke(0);

        var relativeSaberPaths = GetDistinctSaberFiles(true);
        Logger.Info($"Found {relativeSaberPaths.Length} saber files in CustomSabers");

        for (var i = 0; i < relativeSaberPaths.Length; i++)
        {
            var saberFilePath = relativeSaberPaths[i];

            if (existingFileMetadata.ContainsKey(saberFilePath))
            {
                cachedMetadata.Add(existingFileMetadata[saberFilePath]);
                continue;
            }

            var result = await customSabersLoader.LoadSaberDataAsync(saberFilePath);
            using var saberData = result.saberData;
            var loadingError = result.loadingError;

            var metadata = saberData.FilePath != null
                ? new CustomSaberMetadata {
                    SaberName = saberData.Descriptor.SaberName,
                    AuthorName = saberData.Descriptor.AuthorName,
                    RelativePath = saberData.FilePath,
                    MissingShaders = saberData.MissingShaders,
                    CoverImage = saberData.Descriptor.CoverImage?.texture.DuplicateTexture().Downscale(128, 128).EncodeToPNG(),
                    LoadingError = loadingError,
                }
                : new CustomSaberMetadata {
                    SaberName = Path.GetFileNameWithoutExtension(saberFilePath),
                    LoadingError = loadingError,
                };

            var metaFileName = Path.GetFileNameWithoutExtension(saberFilePath) + FileExts.Metadata;
            var metaFilePath = Path.Combine(cachePath, metaFileName);

            cachedMetadata.Add(
                await WriteMetadataToFileAsync(metadata, metaFilePath) ? metadata
                : metadata with { LoadingError = SaberLoaderError.Unknown });

            var currPercentProgress = (i + 1) * 100 / relativeSaberPaths.Length;
            if (currPercentProgress != prevPercentProgress)
            {
                LoadingProgressChanged?.Invoke(currPercentProgress);
                prevPercentProgress = currPercentProgress;
            }
        }

        return cachedMetadata;
    }

    private async Task<bool> WriteMetadataToFileAsync(CustomSaberMetadata metadata, string metaFilePath)
    {
        try
        {
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
            Logger.Error($"Problem encountered when trying to cache a saber's metadata\n{metadata.RelativePath}\n{ex}");
            return false;
        }
    }

    private void ClearCache()
    {
        Logger.Debug("Clearing all cached metadata");

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

    private string[] GetDistinctSaberFiles(bool returnShortPath) =>
        FileUtils.GetFilePaths(sabersPath, [FileExts.Saber, FileExts.Whacker], SearchOption.AllDirectories, returnShortPath);

    private string[] GetMetadataFiles(bool returnShortPath) =>
        FileUtils.GetFilePaths(cachePath, [FileExts.Metadata], SearchOption.TopDirectoryOnly, returnShortPath);
}
