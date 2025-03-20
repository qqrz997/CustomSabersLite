using System.Collections.Generic;
using UnityEngine;

namespace CustomSabersLite.Services;

internal class SpriteCache
{
    private readonly Dictionary<string, Sprite> cache = [];

    public void AddSprite(string saberHash, Sprite? sprite)
    {
        if (sprite != null)
        {
            cache.TryAdd(saberHash, sprite);
        }
    }

    public Sprite? GetSprite(string relativePath) => cache.GetValueOrDefault(relativePath);
}
