using System;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;

namespace CustomSabersLite.Utilities.Common;

internal class CSLResources
{
    private const string ResourcesPath = "CustomSabersLite.Resources.";
    
    public static Sprite NullCoverImage { get; } = LoadSpriteResource("null-image.png");
    public static Sprite DefaultCoverImage { get; } = LoadSpriteResource("defaultsabers-image.jpg");
    public static Sprite ExtremeArrowIcon { get; } = LoadSpriteResource("extreme-arrow-icon.png");
    public static Sprite FolderIcon { get; } = LoadSpriteResource("folder.png");
    public static Sprite FolderFavouritesIcon { get; } = LoadSpriteResource("folder-favourites.png");
    public static Sprite SortAscendingIcon { get; } = LoadSpriteResource("sort-ascending.png");
    public static Sprite SortDescendingIcon { get; } = LoadSpriteResource("sort-descending.png");
    public static Sprite PreviewStaticIcon { get; } = LoadSpriteResource("preview-static.png");
    public static Sprite PreviewHeldIcon { get; } = LoadSpriteResource("preview-held.png");
    public static Sprite EllipsisIcon { get; } = LoadSpriteResource("ellipsis.png");
    public static Sprite FavouriteBlockedIcon { get; } = LoadSpriteResource("favourite-blocked.png");
    public static Sprite CustomTrailIcon { get; } = LoadSpriteResource("custom-trail.png");
    public static Sprite NoTrailIcon { get; } = LoadSpriteResource("no-trail.png");
    

    private static Sprite LoadSpriteResource(string resourceName) =>
        new Texture2D(2, 2).ToSprite(ResourceLoading.GetResource(ResourcesPath + resourceName))
        ?? throw new InvalidOperationException("Failed to create a sprite from an internal image");
}
