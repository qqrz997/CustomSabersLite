using System;
using UnityEngine;
using Logger = CustomSabersLite.Logger;

namespace CustomSabersLite.Models;

internal class CustomSaberData(ISaberMetadata metadata, AssetBundle assetBundle, SaberPrefab saberPrefab) : ISaberData
{
    public ISaberMetadata Metadata { get; } = metadata;
    private AssetBundle AssetBundle { get; } = assetBundle;
    public SaberPrefab Prefab { get; } = saberPrefab;

    public void Dispose(bool unloadAllLoadedObjects)
    {
        if (unloadAllLoadedObjects) Prefab.Dispose();
        if (AssetBundle) AssetBundle.Unload(unloadAllLoadedObjects);
    }

    public void Dispose()
    {
        try
        {
            Prefab.Dispose();
            if (AssetBundle != null) AssetBundle.Unload(true);
        }
        catch (Exception ex)
        {
            Logger.Error($"Couldn't dispose data for saber asset\n{ex}");
        }
    }
}
