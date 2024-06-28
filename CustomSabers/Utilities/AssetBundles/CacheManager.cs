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

    public event Action CacheInitializationFinished;
    
    public bool InitializationFinished { get; private set; }

    public bool InitializationFailed { get; private set; }

    public int SelectedSaberIndex { get; set; } = 0; // used by UI to get position on the saber list 

    public List<CustomSaberMetadata> SabersMetadata { get; private set; } = [];

    private readonly string[] metaExt = [FileExts.Metadata];
    private readonly string[] saberExt = [FileExts.Saber, FileExts.Whacker];

    public async void Initialize()
    {
        if (config.PluginVer != Plugin.Version.ToString())
        {
            ClearCache();
            config.PluginVer = Plugin.Version.ToString();
        }

        try
        {
            await LoadAsync();
        }
        catch (Exception ex)
        {
            InitializationFailed = true;
            Logger.Warn($"{ex}");
        }

        InitializationFinished = true;
        CacheInitializationFinished?.Invoke();
    }

    public async Task ReloadAsync() => await LoadAsync();

    private async Task LoadAsync()
    {
        Logger.Debug("Initializing caching step");
        Logger.Debug(config.CurrentlySelectedSaber != null ? $"Selected saber is: {config.CurrentlySelectedSaber}" : string.Empty);

        var existingFileMetadata = GetMetadataFiles(false)
            .Select(File.ReadAllText)
            .Select(JsonConvert.DeserializeObject<CustomSaberMetadata>)
            .Where(metadata => metadata.RelativePath != null && File.Exists(Path.Combine(sabersPath, metadata.RelativePath)))
            .ToDictionary(metadata => metadata.RelativePath);

        var cachedMetadata = await UpdateCache(existingFileMetadata);

        var sw = Stopwatch.StartNew();
        var regex = new Regex(@"<color=#F77>Not loaded - </color> |<[^>]*>");
        var sortedMetadata = cachedMetadata.OrderBy(v => regex.Replace(v.SaberName, string.Empty));
        sw.Stop();
        Logger.Info($"Sorting took {sw.Elapsed}");

        SabersMetadata.Clear();
        SabersMetadata.Add(new CustomSaberMetadata() { SaberName = "Default", AuthorName = "Beat Games" });
        SabersMetadata.AddRange(sortedMetadata);

        var index = SabersMetadata.FindIndex(metadata => metadata.RelativePath == config.CurrentlySelectedSaber);
        SelectedSaberIndex = index < 0 ? 0 : index;
    }

    private async Task<List<CustomSaberMetadata>> UpdateCache(Dictionary<string, CustomSaberMetadata> existingFileMetadata)
    {
        var cachedMetadata = new List<CustomSaberMetadata>();
        foreach (var saberFilePath in GetSaberFiles(true))
        {
            if (existingFileMetadata.ContainsKey(saberFilePath))
            {
                cachedMetadata.Add(existingFileMetadata[saberFilePath]);
                continue;
            }

            // basic blacklist implementation
            if (SaberAssetBlacklist.IsOnBlacklist(saberFilePath))
            {
                cachedMetadata.Add(new CustomSaberMetadata 
                { 
                    SaberName = $"<color=#F77>Not loaded - </color> {Path.GetFileNameWithoutExtension(saberFilePath)}",
                    AuthorName = "Incompatible with current Beat Saber version",
                    RelativePath = null,
                });
                continue;
            }

            using var saberData = await customSabersLoader.LoadSaberDataAsync(saberFilePath);
            var metadata = new CustomSaberMetadata
            {
                SaberName = saberData.Descriptor.SaberName,
                AuthorName = saberData.Descriptor.AuthorName,
                RelativePath = saberData.FilePath,
                MissingShaders = saberData.MissingShaders,
                CoverImage = saberData.Descriptor.CoverImage?.texture.DuplicateTexture().Downscale(128, 128).EncodeToPNG(),
            };

            var metaFileName = Path.GetFileNameWithoutExtension(metadata.RelativePath) + FileExts.Metadata;
            var metaFilePath = Path.Combine(cachePath, metaFileName);

            if (await WriteMetadataToFileAsync(metadata, metaFilePath))
                cachedMetadata.Add(metadata);
        }

        return cachedMetadata;
    }

    private List<string> GetSaberFiles(bool returnShortPath) =>
        FileUtils.GetFilePaths(sabersPath, saberExt, searchOption: SearchOption.AllDirectories, returnShortPath);

    private List<string> GetMetadataFiles(bool returnShortPath) =>
        FileUtils.GetFilePaths(cachePath, metaExt, searchOption: SearchOption.TopDirectoryOnly, returnShortPath);

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
}
