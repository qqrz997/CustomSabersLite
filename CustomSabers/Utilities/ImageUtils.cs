using UnityEngine;

namespace CustomSabersLite.Utilities
{
    internal static class ImageUtils
    {
        public static readonly Sprite nullCoverImage = ImageLoading.LoadSpriteFromResources("CustomSabersLite.Resources.null-image.png");
        public static readonly Sprite defaultCoverImage = ImageLoading.LoadSpriteFromResources("CustomSabersLite.Resources.defaultsabers-image.png");

        /// <summary>
        /// Duplicates a texture to get around unreadable assets
        /// </summary>
        public static Texture2D DuplicateTexture(this Texture2D source)
        {
            if (source == null) return null;
            RenderTexture renderTex = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
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

        /// <summary>
        /// Creates a <seealso cref="Sprite"/> from raw image data
        /// </summary>
        public static Sprite LoadImage(this byte[] imageData)
        {
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imageData);
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
        }
    }
}
