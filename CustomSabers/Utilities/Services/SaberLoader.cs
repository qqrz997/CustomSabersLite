using System.IO;
using System.Threading.Tasks;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;

namespace CustomSabersLite.Utilities.Services;

internal class SaberLoader(SpriteCache spriteCache, ITimeService timeService)
{
    private readonly SpriteCache spriteCache = spriteCache;
    private readonly ITimeService timeService = timeService;
    private readonly string sabersPath = PluginDirs.CustomSabers.FullName;

    private const CustomSaberType Type = CustomSaberType.Saber;

    /// <summary>
    /// Loads a custom saber from a .saber file
    /// </summary>
    public async Task<ISaberData> LoadCustomSaberAsync(string relativePath)
    {
        string path = Path.Combine(sabersPath, relativePath);

        if (!File.Exists(path))
            return new NoSaberData(relativePath, timeService.GetUtcTime(), SaberLoaderError.FileNotFound);

        Logger.Debug($"Attempting to load saber file - {relativePath}");

        using var fileStream = File.OpenRead(path);

        var bundle = await BundleLoading.LoadBundle(fileStream);

        if (bundle == null)
            return new NoSaberData(relativePath, timeService.GetUtcTime(), SaberLoaderError.NullBundle);

        var saberPrefab = await BundleLoading.LoadAsset<GameObject>(bundle, "_CustomSaber");

        if (saberPrefab == null)
        {
            bundle.Unload(true);
            return new NoSaberData(relativePath, timeService.GetUtcTime(), SaberLoaderError.NullAsset);
        }

        var descriptor = saberPrefab.GetComponent<SaberDescriptor>();

        saberPrefab.hideFlags |= HideFlags.DontUnloadUnusedAsset;
        saberPrefab.name += $" {descriptor.SaberName}";

        var icon = descriptor.CoverImage;
        if (icon != null && icon.texture != null)
            icon = icon.texture.DuplicateTexture().Downscale(128, 128).ToSprite();
        spriteCache.AddSprite(relativePath, icon);

        #if SHADER_DEBUG
        await ShaderInfoDump.Instance.RegisterModelShaders(saberPrefab, descriptor.SaberName ?? "Unknown Saber");
        #else
        await ShaderRepairUtils.RepairSaberShadersAsync(saberPrefab);
        #endif

        string? assetHash = await Task.Run(() => Hashing.MD5Checksum(path, "x2"));

        return
            new CustomSaberData(
                new CustomSaberMetadata(
                    new SaberFileInfo(path, assetHash, timeService.GetUtcTime(), Type),
                    SaberLoaderError.None,
                    new Descriptor(descriptor.SaberName, descriptor.AuthorName, icon)),
                bundle,
                new(saberPrefab, Type));
    }
}
