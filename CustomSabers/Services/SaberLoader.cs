using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;

namespace CustomSabersLite.Services;

internal class SaberLoader
{
    private readonly SpriteCache spriteCache;
    private readonly FavouritesManager favouritesManager;

    public SaberLoader(
        SpriteCache spriteCache,
        FavouritesManager favouritesManager)
    {
        this.spriteCache = spriteCache;
        this.favouritesManager = favouritesManager;
    }

    /// <summary>
    /// Loads a custom saber from a .saber file
    /// </summary>
    public async Task<ISaberData> LoadCustomSaberAsync(SaberFileInfo saberFile)
    {
        AssetBundle? bundle = null;
        GameObject? saberPrefab = null;
        
        try
        {
            if (!saberFile.FileInfo.Exists)
            {
                return new NoSaberData(saberFile, SaberLoaderError.FileNotFound);
            }

            Logger.Debug($"Attempting to load saber file - {saberFile.FileInfo.Name}");

            await using var fileStream = File.OpenRead(saberFile.FileInfo.FullName);

            bundle = await BundleLoading.LoadBundle(fileStream);

            if (bundle == null)
            {
                return new NoSaberData(saberFile, SaberLoaderError.NullBundle);
            }

            saberPrefab = await BundleLoading.LoadAsset<GameObject>(bundle, "_CustomSaber");

            if (saberPrefab == null)
            {
                bundle.Unload(true);
                return new NoSaberData(saberFile, SaberLoaderError.NullAsset);
            }

            var saberDescriptor = saberPrefab.GetComponent<SaberDescriptor>();

            saberPrefab.hideFlags |= HideFlags.DontUnloadUnusedAsset;
            saberPrefab.name += $" {saberDescriptor.SaberName}";

            var icon = saberDescriptor.CoverImage;
            if (icon != null && icon.texture != null)
            {
                icon = icon.texture.DuplicateTexture().Downscale(128, 128).ToSprite();
            }

            spriteCache.AddSprite(saberFile.Hash, icon);

            #if SHADER_DEBUG
            await ShaderInfoDump.Instance.RegisterModelShaders(saberPrefab, descriptor.SaberName ?? "Unknown Saber");
            #else
            await ShaderRepairUtils.RepairSaberShadersAsync(saberPrefab);
            #endif
            
            var saberName = RichTextString.Create(saberDescriptor.SaberName);
            var authorName = RichTextString.Create(saberDescriptor.AuthorName);
            var saberIcon = icon != null ? icon : CSLResources.NullCoverImage;
            var descriptor = new Descriptor(saberName, authorName, saberIcon);
            var hasTrails = CustomTrailUtils.GetTrailsFromCustomSaber(saberPrefab).Any();
            var isFavourite = favouritesManager.IsFavourite(saberFile);
            var metadata = new CustomSaberMetadata(saberFile, SaberLoaderError.None, descriptor, hasTrails, isFavourite); 
            var customSaberPrefab = new CustomSaberPrefab(saberPrefab);
            return new CustomSaberData(metadata, bundle, customSaberPrefab);
        }
        catch
        {
            if (bundle != null) bundle.Unload(true);
            throw;
        }
        finally
        {
            if (saberPrefab != null) saberPrefab.hideFlags &= ~HideFlags.DontUnloadUnusedAsset;
        }
    }
}
