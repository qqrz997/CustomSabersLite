using UnityEngine;

namespace CustomSabersLite.Models;

internal sealed class SpriteThumbnail(Sprite sprite) : IThumbnail
{
    private readonly Sprite sprite = sprite;
    public Sprite GetSprite() => sprite;
}
