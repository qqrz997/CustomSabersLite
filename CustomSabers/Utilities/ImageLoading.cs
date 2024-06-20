using System.Linq;
using UnityEngine;

namespace CustomSabersLite.Utilities;

internal class ImageLoading
{
    public static Sprite LoadSpriteFromResources(string resourcePath, float pixelsPerUnit = 100.0f) =>
        LoadSpriteRaw(ResourceLoading.LoadFromResourceAsync(resourcePath).Result, pixelsPerUnit);

    public static Sprite LoadSpriteRaw(byte[] image, float pixelsPerUnit = 100.0f) =>
        image != null ? LoadSpriteFromTexture(LoadTextureRaw(image), pixelsPerUnit) 
        : null;

    public static Texture2D LoadTextureRaw(byte[] file)
    {
        if (file.Count() > 0)
        {
            var texture = new Texture2D(2, 2);
            if (texture.LoadImage(file))
            {
                return texture;
            }
        }
        return null;
    }

    public static Sprite LoadSpriteFromTexture(Texture2D spriteTexture, float pixelsPerUnit = 100.0f) =>
        spriteTexture ? Sprite.Create(spriteTexture, new(0, 0, spriteTexture.width, spriteTexture.height), new(0, 0), pixelsPerUnit)
        : null;
}
