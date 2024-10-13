using UnityEngine;

namespace CustomSabersLite.Utilities;

internal static class TextureExtensions
{
    /// <summary>
    /// Duplicates a texture to get around unreadable assets
    /// </summary>
    public static Texture2D DuplicateTexture(this Texture2D source)
    {
        var renderTex = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Default);
        Graphics.Blit(source, renderTex);
        var previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        var readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }

    /// <summary>
    /// Downscales a texture if it is bigger than the given width and height
    /// </summary>
    public static Texture2D Downscale(this Texture2D origTexture, int width, int height, FilterMode filterMode = FilterMode.Trilinear) =>
        width * height > origTexture.width * origTexture.height ? origTexture
        : origTexture.Rescale(width, height, filterMode);

    private static Texture2D Rescale(this Texture2D origTexture, int width, int height, FilterMode filterMode)
    {
        var textureRect = new Rect(0, 0, width, height);

        origTexture.filterMode = filterMode;
        origTexture.Apply(true);
        var renderTexture = new RenderTexture(width, height, 32);
        Graphics.SetRenderTarget(renderTexture);
        GL.LoadPixelMatrix(0, 1, 1, 0);
        GL.Clear(true, true, Color.clear);
        Graphics.DrawTexture(new Rect(0, 0, 1, 1), origTexture);

        origTexture.Reinitialize(width, height);
        origTexture.ReadPixels(textureRect, 0, 0, true);
        origTexture.Apply(true);

        return origTexture;
    }

    /// <summary>
    /// Creates a <seealso cref="Sprite"/> from raw image data
    /// </summary>
    public static Sprite? ToSprite(this Texture2D tex, byte[] imageData, float pixelsPerUnit = 100) =>
        !tex.LoadImage(imageData) ? null
        : Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);

    /// <summary>
    /// Creates a <seealso cref="Sprite"/> from an existing texture
    /// </summary>
    public static Sprite? ToSprite(this Texture2D? tex, float pixelsPerUnit = 100) =>
        !tex || tex is null ? null
        : Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), pixelsPerUnit);
}
