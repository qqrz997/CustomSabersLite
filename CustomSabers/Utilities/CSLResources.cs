using System.Linq;
using UnityEngine;

using static CustomSabersLite.Utilities.ImageLoading;

namespace CustomSabersLite.Utilities;

internal class CSLResources
{
    public static Sprite NullCoverImage { get; } = 
        LoadSpriteResource("CustomSabersLite.Resources.null-image.png").Result;

    public static Sprite DefaultCoverImage { get; } = 
        LoadSpriteResource("CustomSabersLite.Resources.defaultsabers-image.png").Result;

    public static Sprite Fallback { get; } = Resources.FindObjectsOfTypeAll<Sprite>().First();
}
