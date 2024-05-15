using System.Linq;
using UnityEngine;

namespace CustomSabersLite.Utilities
{
    internal class ImageLoading
    {
        public static Sprite LoadSpriteFromResources(string resourcePath, float pixelsPerUnit = 100.0f) =>
            LoadSpriteRaw(ResourceLoading.LoadFromResourceAsync(resourcePath).Result, pixelsPerUnit);

        public static Sprite LoadSpriteRaw(byte[] image, float pixelsPerUnit = 100.0f)
        {
            if (image != null) return LoadSpriteFromTexture(LoadTextureRaw(image), pixelsPerUnit);
            else return null;
        }

        public static Texture2D LoadTextureRaw(byte[] file)
        {
            if (file.Count() > 0)
            {
                Texture2D texture = new Texture2D(2, 2);
                if (texture.LoadImage(file))
                {
                    return texture;
                }
            }
            return null;
        }

        public static Sprite LoadSpriteFromTexture(Texture2D spriteTexture, float pixelsPerUnit = 100.0f)
        {
            if (spriteTexture)
            {
                return Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), new Vector2(0, 0), pixelsPerUnit);
            }
            return null;
        }
    }
}
