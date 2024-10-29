using System;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class CustomSaberData(ISaberMetadata metadata, AssetBundle assetBundle, GameObject saberPrefab) : ISaberData
{
    public ISaberMetadata Metadata { get; } = metadata;

    private AssetBundle AssetBundle { get; } = assetBundle;

    public SaberPrefab? Prefab { get; } = SaberPrefab.TryCreate(saberPrefab);

    public GameObject? GetPrefab(SaberType saberType) =>
        Prefab == null ? null
        : saberType == SaberType.SaberA ? Prefab.Left : Prefab.Right;

    public void Dispose(bool unloadAllLoadedObjects)
    {
        if (unloadAllLoadedObjects) Prefab?.Dispose();
        if (AssetBundle) AssetBundle.Unload(unloadAllLoadedObjects);
    }

    public void Dispose()
    {
        try
        {
            Prefab?.Dispose();
            if (AssetBundle) AssetBundle.Unload(true);
        }
        catch (Exception ex)
        {
            Logger.Error($"Couldn't dispose data for saber asset\n{ex}");
        }
    }
}
