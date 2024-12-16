using System;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;

namespace CustomSabersLite.Models;

// Encapsulates a custom saber prefab and manages instantiation calls
internal class SaberPrefab : IDisposable
{
    private readonly GameObject prefab;
    private readonly CustomSaberType customSaberType;

    public SaberPrefab(GameObject prefab, CustomSaberType customSaberType)
    {
        this.prefab = prefab;
        this.customSaberType = customSaberType;
    }

    public SaberInstanceSet Instantiate() => SaberInstanceSet.FromPrefab(prefab, customSaberType);

    public void Dispose()
    {
        if (prefab != null) prefab.Destroy();
    }
}
