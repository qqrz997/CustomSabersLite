using UnityEngine;

namespace CustomSabersLite.Utilities
{
    internal class ImageUtils
    {
        public static readonly Sprite nullCoverImage = ImageLoading.LoadSpriteFromResources("CustomSabersLite.Resources.null-image.png");
        public static readonly Sprite defaultCoverImage = ImageLoading.LoadSpriteFromResources("CustomSabersLite.Resources.defaultsabers-image.png");

        /// <summary>
        /// Duplicates a texture to get around unreadable assets
        /// </summary>
        public static Texture2D DuplicateTexture(Texture2D source)
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
    }
}
