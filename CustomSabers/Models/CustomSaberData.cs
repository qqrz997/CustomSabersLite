using CustomSabersLite.Utilities;
using System;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class CustomSaberData(ISaberMetadata metadata, AssetBundle assetBundle, GameObject saberPrefab) : ISaberData
{
    public ISaberMetadata Metadata { get; } = metadata;

    private AssetBundle AssetBundle { get; } = assetBundle;

    private GameObject ParentPrefab { get; } = saberPrefab;
    private SaberPrefab Left { get; } = new(saberPrefab, SaberType.SaberA);
    private SaberPrefab Right { get; } = new(saberPrefab, SaberType.SaberB);


    public GameObject GetPrefab(SaberType type) =>
        type == SaberType.SaberA ? Left.Prefab : Right.Prefab;
    
    public void Dispose(bool unloadAllLoadedObjects)
    {
        if (unloadAllLoadedObjects) ParentPrefab.Destroy();
        if (AssetBundle) AssetBundle.Unload(unloadAllLoadedObjects);
    }

    public void Dispose()
    {
        try
        {
            ParentPrefab.Destroy();
            if (AssetBundle) AssetBundle.Unload(true);
        }
        catch (Exception ex)
        {
            Logger.Error($"Couldn't dispose data for saber asset {Metadata.FileInfo}\n{ex}");
        }
    }

}
