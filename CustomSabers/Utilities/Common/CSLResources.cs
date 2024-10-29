using System;
using UnityEngine;

namespace CustomSabersLite.Utilities;

internal class CSLResources
{
    public static Sprite NullCoverImage { get; } = LoadSpriteResource("CustomSabersLite.Resources.null-image.png");

    public static Sprite DefaultCoverImage { get; } = LoadSpriteResource("CustomSabersLite.Resources.defaultsabers-image.jpg");

    public static Sprite ExtremeArrowIcon { get; } = LoadSpriteResource("CustomSabersLite.Resources.extreme-arrow-icon.png");

    private static Sprite LoadSpriteResource(string resourceName) =>
        new Texture2D(2, 2).ToSprite(ResourceLoading.GetResource(resourceName))
        ?? throw new InvalidOperationException("Failed to create a sprite from an internal image");
}
