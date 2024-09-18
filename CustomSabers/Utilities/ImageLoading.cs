using System.Threading.Tasks;
using UnityEngine;

using static CustomSabersLite.Utilities.ResourceLoading;

namespace CustomSabersLite.Utilities;

internal class ImageLoading
{
    public static async Task<Sprite> LoadSpriteResource(string resourceName) =>
        new Texture2D(2, 2).ToSprite(await LoadFromResourceAsync(resourceName)) 
        ?? CSLResources.Fallback;
}
