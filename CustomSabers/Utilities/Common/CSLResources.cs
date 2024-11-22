using System;
using UnityEngine;

namespace CustomSabersLite.Utilities;

internal class CSLResources
{
    private const string ResourcesPath = "CustomSabersLite.Resources.";
    
    public static Sprite NullCoverImage { get; } = LoadSpriteResource("null-image.png");
    public static Sprite DefaultCoverImage { get; } = LoadSpriteResource("defaultsabers-image.jpg");
    public static Sprite ExtremeArrowIcon { get; } = LoadSpriteResource("extreme-arrow-icon.png");
    public static Sprite TrailDurationIcon { get; } = LoadSpriteResource("trail-duration.png");
    public static Sprite TrailWidthIcon { get; } = LoadSpriteResource("trail-width.png");

    private static Sprite LoadSpriteResource(string resourceName) =>
        new Texture2D(2, 2).ToSprite(ResourceLoading.GetResource(ResourcesPath + resourceName))
        ?? throw new InvalidOperationException("Failed to create a sprite from an internal image");
}
