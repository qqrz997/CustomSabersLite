using System;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class CustomSaberData : ISaberData
{
    public CustomSaberData(CustomSaberMetadata metadata, AssetBundle assetBundle, ISaberPrefab customSaberPrefab)
    {
        Metadata = metadata;
        AssetBundle = assetBundle;
        Prefab = customSaberPrefab;
    }

    public CustomSaberMetadata Metadata { get; }
    private AssetBundle AssetBundle { get; }
    public ISaberPrefab Prefab { get; }

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
