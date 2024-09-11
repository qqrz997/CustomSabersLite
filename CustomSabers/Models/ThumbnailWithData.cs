using CustomSabersLite.Utilities;
using UnityEngine;

namespace CustomSabersLite.Models;

internal sealed record ThumbnailWithData(byte[] ImageData) : IThumbnail
{
    private Sprite Sprite { get; set; }

    public Sprite GetSprite() =>
        Sprite ??= new Texture2D(2, 2).ToSprite(ImageData);
}
