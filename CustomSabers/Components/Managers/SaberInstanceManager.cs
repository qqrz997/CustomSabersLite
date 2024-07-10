using CustomSabersLite.Data;
using System;
using System.Collections.Generic;

namespace CustomSabersLite.Components.Managers;

internal class SaberInstanceManager : IDisposable
{
    private readonly Dictionary<string, CustomSaberData> saberInstances = [];

    public void AddSaber(CustomSaberData saberData)
    {
        if (saberData.FilePath is null)
        {
            return;
        }

        if (!HasSaber(saberData.FilePath))
        {
            saberInstances.Add(saberData.FilePath, saberData);
        }
    }

    public bool HasSaber(string saberPath) => 
        saberInstances.ContainsKey(saberPath);

    public CustomSaberData TryGetSaber(string saberPath) => 
        saberPath is null ? null
        : !saberInstances.ContainsKey(saberPath) ? null
        : saberInstances[saberPath];

    public void Dispose()
    {
        foreach (var saberInstance in saberInstances.Values)
        {
            saberInstance.Dispose();
        }
    }
}
