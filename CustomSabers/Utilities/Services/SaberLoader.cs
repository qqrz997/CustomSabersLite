using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;

namespace CustomSabersLite.Utilities;

/// <summary>
/// Class for loading .saber files
/// </summary>
internal class SaberLoader(SpriteCache spriteCache)
{
    private readonly SpriteCache spriteCache = spriteCache;
    private readonly string sabersPath = PluginDirs.CustomSabers.FullName;

    private const CustomSaberType Type = CustomSaberType.Saber;

    /// <summary>
    /// Loads a custom saber from a .saber file
    /// </summary>
    /// <param name="relativePath">Path to the .saber file in the CustomSabers folder</param>
    /// <returns><seealso cref="NoSaberData"/> if a custom saber failed to load</returns>
    public async Task<ISaberData> LoadCustomSaberAsync(string relativePath)
    {
        var path = Path.Combine(sabersPath, relativePath);

        if (!File.Exists(path))
            return new NoSaberData(relativePath, SaberLoaderError.FileNotFound);

        Logger.Debug($"Attempting to load saber file - {relativePath}");

        using var fileStream = File.OpenRead(path);

        var bundle = await BundleLoading.LoadBundle(fileStream);

        if (bundle == null)
            return new NoSaberData(relativePath, SaberLoaderError.NullBundle);

        var saberPrefab = await BundleLoading.LoadAsset<GameObject>(bundle, "_CustomSaber");

        if (saberPrefab == null)
        {
            bundle.Unload(true);
            return new NoSaberData(relativePath, SaberLoaderError.NullAsset);
        }

        var descriptor = saberPrefab.GetComponent<SaberDescriptor>();

        saberPrefab.hideFlags |= HideFlags.DontUnloadUnusedAsset;
        saberPrefab.name += $" {descriptor.SaberName}";

        var icon = descriptor.CoverImage;
        if (icon != null && icon.texture != null)
            icon = icon.texture.DuplicateTexture().Downscale(128, 128).ToSprite();
        spriteCache.AddSprite(relativePath, icon);

        var shaderInfo = await ShaderRepairUtils.RepairSaberShadersAsync(saberPrefab);
        var missingShaders = !shaderInfo.AllShadersReplaced;
        var missingShaderNames = shaderInfo.MissingShaderNames;

        var assetHash = await Task.Run(() => Hashing.MD5Checksum(path, "x2")) ?? string.Empty;

        return
            new CustomSaberData(
                new CustomSaberMetadata(
                    new SaberFileInfo(relativePath, assetHash, Type),
                    SaberLoaderError.None,
                    new Descriptor(descriptor.SaberName, descriptor.AuthorName, icon),
                    new SaberModelFlags(missingShaders, missingShaderNames)),
                bundle,
                saberPrefab);
    }
}
