using CustomSabersLite.Models;
using CustomSabersLite.Utilities;
using System;
using System.Collections.Generic;

namespace CustomSabersLite.Components.Managers;

internal class SaberInstanceManager() : IDisposable
{
    private readonly Dictionary<string, CustomSaberData> saberInstances = [];

    public void AddSaber(CustomSaberData saberData) =>
        saberInstances.TryAdd(saberData.Metadata.FileInfo.RelativePath, saberData);

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
