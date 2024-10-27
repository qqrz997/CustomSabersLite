using CustomSabersLite.Models;
using System;
using System.Collections.Generic;

namespace CustomSabersLite.Utilities.Services;

internal class SaberInstanceManager() : IDisposable
{
    private readonly Dictionary<string, CustomSaberData> saberInstances = [];

    public void AddSaber(CustomSaberData saberData)
    {
        if (saberData.Metadata.SaberFile.RelativePath != null)
        {
            saberInstances.TryAdd(saberData.Metadata.SaberFile.RelativePath, saberData);
        }
    }

    public bool HasSaber(string saberPath) =>
        saberInstances.ContainsKey(saberPath);

    public CustomSaberData? TryGetSaber(string? saberPath) =>
        saberPath is null ? null
        : !saberInstances.ContainsKey(saberPath) ? null
        : saberInstances[saberPath];

    public void Dispose() => Clear(true);

    public void Clear(bool unloadAllLoadedObjects)
    {
        saberInstances.Values.ForEach(i => i.Dispose(unloadAllLoadedObjects));
        saberInstances.Clear();
    }
}
