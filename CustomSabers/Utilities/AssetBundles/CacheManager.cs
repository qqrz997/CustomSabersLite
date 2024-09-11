using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using CustomSabersLite.UI.Managers;
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

internal class CacheManager : IInitializable
{
    private readonly CSLConfig config;
    private readonly CustomSabersLoader customSabersLoader;
    private readonly PluginDirs directories;
    private readonly SaberListManager saberListManager;

    private CacheManager(PluginDirs pluginDirs, CSLConfig config, CustomSabersLoader customSabersLoader, SaberListManager saberListManager)
    {
        this.config = config;
        this.customSabersLoader = customSabersLoader;
        directories = pluginDirs;
        this.saberListManager = saberListManager;
    }

    public event Action<int> LoadingProgressChanged;
    public event Action LoadingComplete;
    
    public bool InitializationFinished { get; private set; }

    public bool InitializationFailed { get; private set; }

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
        saberListManager.SetData([]);

        Logger.Debug("Initializing caching step");
        Logger.Debug(config.CurrentlySelectedSaber != null
            ? $"Selected saber is: {config.CurrentlySelectedSaber}"
            : "No custom saber selected");

        var metaFilePaths = GetMetadataFiles(false);
        Logger.Debug($"Found {metaFilePaths.Length} meta files in cache");
        var existingFileMetadata = metaFilePaths
            .Select(File.ReadAllText)
            .Select(JsonConvert.DeserializeObject<CustomSaberMetadata>)
            .Where(metadata => metadata.RelativePath != null && File.Exists(Path.Combine(directories.CustomSabers.FullName, metadata.RelativePath)))
            .ToDictionary(metadata => metadata.RelativePath);

        var cachedMetadata = await UpdateCacheAsync(existingFileMetadata);
        Logger.Info($"Obtained metadata for {cachedMetadata.Count} sabers");

        saberListManager.SetData(cachedMetadata);
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
            var metaFilePath = Path.Combine(directories.Cache.FullName, metaFileName);

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
            var destinationPath = Path.Combine(directories.DeletedSabers.FullName, fileName);

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
        FileUtils.GetFilePaths(directories.CustomSabers.FullName, [FileExts.Saber, FileExts.Whacker], SearchOption.AllDirectories, returnShortPath);

    private string[] GetMetadataFiles(bool returnShortPath) =>
        FileUtils.GetFilePaths(directories.Cache.FullName, [FileExts.Metadata], SearchOption.TopDirectoryOnly, returnShortPath);
}
