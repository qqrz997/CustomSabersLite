using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using Newtonsoft.Json;
using UnityEngine;

namespace CustomSabersLite.Services;

internal class WhackerLoader
{
    private readonly SpriteCache spriteCache;
    private readonly FavouritesManager favouritesManager;

    public WhackerLoader(
        SpriteCache spriteCache,
        FavouritesManager favouritesManager)
    {
        this.spriteCache = spriteCache;
        this.favouritesManager = favouritesManager;
    }

    /// <summary>
    /// Loads a custom saber from a .whacker file
    /// </summary>
    public async Task<ISaberData> LoadWhackerAsync(SaberFileInfo saberFile)
    {
        if (!saberFile.FileInfo.Exists)
            return new NoSaberData(saberFile, SaberLoaderError.FileNotFound);

        Logger.Debug($"Attempting to load whacker file - {saberFile.FileInfo.Name}");

        using var fileStream = saberFile.FileInfo.OpenRead();
        using var archive = new ZipArchive(fileStream, ZipArchiveMode.Read);

        var jsonEntry = archive.Entries.FirstOrDefault(x => x.FullName.EndsWith(".json"));
        
        if (jsonEntry is null)
            return new NoSaberData(saberFile, SaberLoaderError.FileNotFound);
        
        using var jsonStream = jsonEntry.Open();
        using var jsonStreamReader = new StreamReader(jsonStream);

        if (new JsonSerializer().Deserialize(jsonStreamReader, typeof(WhackerModel)) is not WhackerModel whacker)
            return new NoSaberData(saberFile, SaberLoaderError.InvalidFileType);

        if (whacker.Config.IsLegacy)
            return new NoSaberData(saberFile, SaberLoaderError.LegacyWhacker);

        var bundleEntry = archive.GetEntry(whacker.FileName);

        if (bundleEntry is null)
            return new NoSaberData(saberFile, SaberLoaderError.FileNotFound);
        
        using var bundleStream = bundleEntry.Open();
        var bundle = await BundleLoading.LoadBundle(bundleStream);

        if (bundle == null)
            return new NoSaberData(saberFile, SaberLoaderError.NullBundle);

        var saberPrefab = await BundleLoading.LoadAsset<GameObject>(bundle, "_Whacker");

        if (saberPrefab == null)
        {
            bundle.Unload(true);
            return new NoSaberData(saberFile, SaberLoaderError.NullAsset);
        }

        saberPrefab.hideFlags |= HideFlags.DontUnloadUnusedAsset;
        saberPrefab.name += $" {whacker.Descriptor.Name}";

        var icon = await GetDownscaledIcon(archive, whacker);
        spriteCache.AddSprite(saberFile.Hash, icon);

        #if SHADER_DEBUG
        await ShaderInfoDump.Instance.RegisterModelShaders(saberPrefab, whacker.descriptor.objectName ?? "Unknown Whacker");
        #else
        await ShaderRepairUtils.RepairSaberShadersAsync(saberPrefab);
        #endif

        var saberName = RichTextString.Create(whacker.Descriptor.Name);
        var authorName = RichTextString.Create(whacker.Descriptor.Author);
        var saberIcon = icon != null ? icon : CSLResources.NullCoverImage;
        var descriptor = new Descriptor(saberName, authorName, saberIcon);
        var hasTrails = CustomTrailUtils.GetTrailsFromWhacker(saberPrefab).Any();
        var isFavourite = favouritesManager.IsFavourite(saberFile);
        var metadata = new CustomSaberMetadata(saberFile, SaberLoaderError.None, descriptor, hasTrails, isFavourite);
        var whackerPrefab = new WhackerPrefab(saberPrefab);
        return new CustomSaberData(metadata, bundle, whackerPrefab);
    }

    private static async Task<Sprite?> GetDownscaledIcon(ZipArchive archive, WhackerModel whacker)
    {
        if (whacker.Descriptor.IconFileName is null) return null;

        var iconEntry = archive.GetEntry(whacker.Descriptor.IconFileName);

        if (iconEntry is null) return null;

        using var memoryStream = new MemoryStream();
        using var thumbStream = iconEntry.Open();
        
        await thumbStream.CopyToAsync(memoryStream);
        
        var icon = new Texture2D(2, 2).ToSprite(memoryStream.ToArray());
        
        return icon == null || icon.texture == null ? null
            : icon.texture.Downscale(128, 128).ToSprite();
    }
}
