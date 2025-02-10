using UnityEngine;

namespace CustomSabersLite.Models;

internal class CustomSaberData : ISaberData
{
    private readonly AssetBundle bundle;
    
    public CustomSaberData(CustomSaberMetadata metadata, AssetBundle assetBundle, ISaberPrefab customSaberPrefab)
    {
        bundle = assetBundle;
        Metadata = metadata;
        Prefab = customSaberPrefab;
    }

    public CustomSaberMetadata Metadata { get; }
    public ISaberPrefab Prefab { get; }

    public void Dispose(bool unloadAllLoadedObjects)
    {
        if (unloadAllLoadedObjects) Prefab.Dispose();
        if (bundle != null) bundle.Unload(unloadAllLoadedObjects);
    }

    public void Dispose()
    {
        Prefab.Dispose();
        if (bundle != null) bundle.Unload(true);
    }
}