using UnityEngine;

namespace CustomSabersLite.Models;

internal class NoThumbnail : IThumbnail
{
    public Sprite? GetSprite() => null;
}
