using System.Collections.Generic;
using UnityEngine;

namespace CustomSabersLite.Utilities;

internal class SpriteCache
{
    private readonly Dictionary<string, Sprite> sprites = [];

    public void AddSprite(string relativePath, Sprite? sprite)
    {
        if (sprite == null) return;
        sprites.TryAdd(relativePath, sprite);
    }

    public Sprite? GetSprite(string relativePath) =>
        sprites.TryGetValue(relativePath, out var sprite) ? sprite : null;

    public bool HasSprite(string relativePath) =>
        sprites.ContainsKey(relativePath);
}
