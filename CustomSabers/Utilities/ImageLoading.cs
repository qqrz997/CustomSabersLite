using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomSaber.Utilities
{
    internal class ImageLoading
    {
        public static Sprite LoadSpriteFromResources(string resourcePath, float pixelsPerUnit = 100.0f)
        {
            return LoadSpriteRaw(LoadFromResource(resourcePath), pixelsPerUnit);
        }

        public static Texture2D DuplicateTexture(Texture2D source)
        {
            if (source == null) return null;
            RenderTexture renderTex = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }

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

        public static byte[] LoadFromResource(string resourcePath)
        {
            return GetResource(Assembly.GetCallingAssembly(), resourcePath);
        }

        public static byte[] GetResource(Assembly assembly, string resourcePath)
        {
            Stream stream = assembly.GetManifestResourceStream(resourcePath);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            return data;
        }
    }
}
