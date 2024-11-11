using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;
using CustomSabersLite.Models;
using UnityEngine;
using System.Threading.Tasks;
using CustomSabersLite.Utilities.Common;

namespace CustomSabersLite.Utilities;

/// <summary>
/// Class for loading .whacker files
/// </summary>
internal class WhackerLoader(SpriteCache spriteCache, ITimeService timeService)
{
    private readonly SpriteCache spriteCache = spriteCache;
    private readonly ITimeService timeService = timeService;
    private readonly string sabersPath = PluginDirs.CustomSabers.FullName;

    private const CustomSaberType Type = CustomSaberType.Whacker;

    /// <summary>
    /// Loads a custom saber from a .whacker file
    /// </summary>
    /// <param name="relativePath">Path to the .whacker file in the CustomSabers folder</param>
    /// <returns><seealso cref="NoSaberData"/> if a custom saber failed to load</returns>
    public async Task<ISaberData> LoadWhackerAsync(string relativePath)
    {
        string? path = Path.Combine(sabersPath, relativePath);

        if (!File.Exists(path))
            return new NoSaberData(relativePath, timeService.GetUtcTime(), SaberLoaderError.FileNotFound);

        Logger.Debug($"Attempting to load whacker file - {relativePath}");

        using var fileStream = File.OpenRead(path);
        using var archive = new ZipArchive(fileStream, ZipArchiveMode.Read);

        var json = archive.Entries.Where(x => x.FullName.EndsWith(".json")).FirstOrDefault();
        using var jsonStream = json.Open();
        using var jsonStreamReader = new StreamReader(jsonStream);

        if (new JsonSerializer().Deserialize(jsonStreamReader, typeof(WhackerModel)) is not WhackerModel whacker)
            return new NoSaberData(relativePath, timeService.GetUtcTime(), SaberLoaderError.InvalidFileType);

        if (whacker.config.isLegacy)
            return new NoSaberData(relativePath, timeService.GetUtcTime(), SaberLoaderError.LegacyWhacker);

        var bundleEntry = archive.GetEntry(whacker.pcFileName);

        using var bundleStream = bundleEntry.Open();
        var bundle = await BundleLoading.LoadBundle(bundleStream);

        if (bundle == null)
            return new NoSaberData(relativePath, timeService.GetUtcTime(), SaberLoaderError.NullBundle);

        var saberPrefab = await BundleLoading.LoadAsset<GameObject>(bundle, "_Whacker");

        if (saberPrefab == null)
        {
            bundle.Unload(true);
            return new NoSaberData(relativePath, timeService.GetUtcTime(), SaberLoaderError.NullAsset);
        }

        saberPrefab.hideFlags |= HideFlags.DontUnloadUnusedAsset;
        saberPrefab.name += $" {whacker.descriptor.objectName}";

        var shaderInfo = await ShaderRepairUtils.RepairSaberShadersAsync(saberPrefab);

        #if SHADER_DEBUG
        shaderInfo.MissingShaderNames.ForEach(n => ShaderInfoDump.Instance.AddShader(n, whacker.descriptor.objectName ?? "Unknown Whacker"));
        #endif

        var icon = await GetDownscaledIcon(archive.GetEntry(whacker.descriptor.coverImage));
        spriteCache.AddSprite(relativePath, icon);

        string? assetHash = await Task.Run(() => Hashing.MD5Checksum(path, "x2")) ?? string.Empty;

        return
            new CustomSaberData(
                new CustomSaberMetadata(
                    new SaberFileInfo(path, assetHash, timeService.GetUtcTime(), Type),
                    SaberLoaderError.None,
                    new Descriptor(whacker.descriptor.objectName, whacker.descriptor.author, icon)),
                bundle,
                saberPrefab);
    }

    private async Task<Sprite?> GetDownscaledIcon(ZipArchiveEntry? thumbEntry)
    {
        if (thumbEntry is null) return null;
        using var memoryStream = new MemoryStream();
        using var thumbStream = thumbEntry.Open();
        await thumbStream.CopyToAsync(memoryStream);
        var icon = new Texture2D(2, 2).ToSprite(memoryStream.ToArray());
        return icon == null || icon.texture == null ? null
            : icon.texture.Downscale(128, 128).ToSprite();
    }
}
