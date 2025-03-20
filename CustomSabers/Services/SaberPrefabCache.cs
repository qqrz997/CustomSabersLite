using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Extensions;

namespace CustomSabersLite.Services;

internal class SaberPrefabCache
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
        if (cache.TryGetValue(saberHash, out var saberData))
        {
            saberData.Dispose(true);
        }
    }

    public void Clear(bool unloadAllLoadedObjects)
    {
        cache.Values.ForEach(i => i.Dispose(unloadAllLoadedObjects));
        cache.Clear();
    }
}
