using UnityEngine;

namespace CustomSabersLite.Models;

internal sealed record ThumbnailWithSprite(Sprite Sprite) : IThumbnail
{
    public Sprite GetSprite() => Sprite;
}
