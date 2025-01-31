using System.IO;
using System.Threading.Tasks;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;

namespace CustomSabersLite.Utilities.Services;

internal class SaberLoader
{
    private readonly SpriteCache spriteCache;
    private readonly ITimeService timeService;

    public SaberLoader(SpriteCache spriteCache, ITimeService timeService)
    {
        this.spriteCache = spriteCache;
        this.timeService = timeService;
    }

    private const CustomSaberType Type = CustomSaberType.Saber;

    /// <summary>
    /// Loads a custom saber from a .saber file
    /// </summary>
    public async Task<ISaberData> LoadCustomSaberAsync(SaberFileInfo saberFile)
    {
        if (!saberFile.FileInfo.Exists)
            return new NoSaberData(saberFile.FileInfo.Name, timeService.GetUtcTime(), SaberLoaderError.FileNotFound);

        Logger.Debug($"Attempting to load saber file - {saberFile.FileInfo.Name}");

        using var fileStream = File.OpenRead(saberFile.FileInfo.FullName);

        var bundle = await BundleLoading.LoadBundle(fileStream);

        if (bundle == null)
            return new NoSaberData(saberFile.FileInfo.Name, timeService.GetUtcTime(), SaberLoaderError.NullBundle);

        var saberPrefab = await BundleLoading.LoadAsset<GameObject>(bundle, "_CustomSaber");

        if (saberPrefab == null)
        {
            bundle.Unload(true);
            return new NoSaberData(saberFile.FileInfo.Name, timeService.GetUtcTime(), SaberLoaderError.NullAsset);
        }

        var descriptor = saberPrefab.GetComponent<SaberDescriptor>();

        saberPrefab.hideFlags |= HideFlags.DontUnloadUnusedAsset;
        saberPrefab.name += $" {descriptor.SaberName}";

        var icon = descriptor.CoverImage;
        if (icon != null && icon.texture != null)
            icon = icon.texture.DuplicateTexture().Downscale(128, 128).ToSprite();
        spriteCache.AddSprite(saberFile.Hash, icon);

        #if SHADER_DEBUG
        await ShaderInfoDump.Instance.RegisterModelShaders(saberPrefab, descriptor.SaberName ?? "Unknown Saber");
        #else
        await ShaderRepairUtils.RepairSaberShadersAsync(saberPrefab);
        #endif

        return
            new CustomSaberData(
                new CustomSaberMetadata(
                    saberFile,
                    SaberLoaderError.None,
                    new(RichTextString.Create(descriptor.SaberName),
                        RichTextString.Create(descriptor.AuthorName),
                        icon != null ? icon : CSLResources.NullCoverImage)),
                bundle,
                new(saberPrefab, Type));
    }
}
