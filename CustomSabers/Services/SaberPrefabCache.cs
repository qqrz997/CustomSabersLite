using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CustomSabersLite.Models;

namespace CustomSabersLite.Services;

internal class SaberPrefabCache : IDisposable
{
    private readonly Dictionary<string, CustomSaberData> cache = [];

    public bool AddSaberPrefab(CustomSaberData saberData) =>
        cache.TryAdd(saberData.Metadata.SaberFile.Hash, saberData);

    public bool TryGetSaberPrefab(string saberHash, [NotNullWhen(true)] out CustomSaberData? saberData)
    {
        saberData = cache.GetValueOrDefault(saberHash);
        return saberData != null;
    }

    public void UnloadSaber(string saberHash)
    {
        if (!cache.TryGetValue(saberHash, out var saberData)) return;
        saberData.Dispose();
        cache.Remove(saberHash);
    }

    public void Dispose()
    {
        Clear();
    }
    
    public void Clear()
    {
        if (cache.Count == 0) return;
        foreach (var customSaberData in cache.Values) customSaberData.Dispose();
        cache.Clear();
    }
}
