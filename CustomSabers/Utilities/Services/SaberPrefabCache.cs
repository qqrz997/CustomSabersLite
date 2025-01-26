using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Extensions;

namespace CustomSabersLite.Utilities.Services;

internal class SaberPrefabCache : IDisposable
{
    private readonly Dictionary<string, CustomSaberData> saberPrefabs = [];

    public void AddSaber(CustomSaberData saberData)
    {
        if (saberData.Metadata.SaberFile.RelativePath != null)
        {
            saberPrefabs.TryAdd(saberData.Metadata.SaberFile.RelativePath, saberData);
        }
    }

    public bool TryGetSaber(string? saberPath, [NotNullWhen(true)] out CustomSaberData? saberData)
    {
        saberData = saberPath is null or [] ? null : saberPrefabs.GetValueOrDefault(saberPath);
        return saberData != null;
    }

    public void Dispose() => Clear(true);
    public void Clear(bool unloadAllLoadedObjects)
    {
        saberPrefabs.Values.ForEach(i => i.Dispose(unloadAllLoadedObjects));
        saberPrefabs.Clear();
    }
}
