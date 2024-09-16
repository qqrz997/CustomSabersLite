using CustomSabersLite.Models;
using CustomSabersLite.Utilities;
using System;
using System.Collections.Generic;

namespace CustomSabersLite.Components.Managers;

internal class SaberInstanceManager : IDisposable
{
    private readonly Dictionary<string, CustomSaberData> saberInstances = [];

    public void AddSaber(CustomSaberData saberData)
    {
        var relativePath = saberData.Metadata.FileInfo.RelativePath;
        if (relativePath is not null && !HasSaber(relativePath))
        {
            saberInstances.Add(relativePath, saberData); ;
        }
    }

    public bool HasSaber(string saberPath) =>
        saberPath is not null && saberInstances.ContainsKey(saberPath);

    public CustomSaberData? TryGetSaber(string? saberPath) =>
        saberPath is null ? null
        : !saberInstances.ContainsKey(saberPath) ? null
        : saberInstances[saberPath];

    public void Dispose() =>
        saberInstances.Values.ForEach(i => i.Dispose());
}
