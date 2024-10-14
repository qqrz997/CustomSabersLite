using CustomSabersLite.Utilities;
using System;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class CustomSaberData(ISaberMetadata metadata, AssetBundle assetBundle, GameObject saberPrefab) : ISaberData
{
    public ISaberMetadata Metadata { get; } = metadata;

    private AssetBundle AssetBundle { get; } = assetBundle;

    private SaberPrefab? SaberPrefab { get; } = SaberPrefab.TryCreate(saberPrefab);

    public GameObject? TryGetPrefab(SaberType type) =>
        type == SaberType.SaberA ? SaberPrefab?.Left : SaberPrefab?.Right;
    
    public void Dispose(bool unloadAllLoadedObjects)
    {
        if (unloadAllLoadedObjects) SaberPrefab?.Dispose();
        if (AssetBundle) AssetBundle.Unload(unloadAllLoadedObjects);
    }

    public void Dispose()
    {
        try
        {
            SaberPrefab?.Dispose();
            if (AssetBundle) AssetBundle.Unload(true);
        }
        catch (Exception ex)
        {
            Logger.Error($"Couldn't dispose data for saber asset {Metadata.FileInfo}\n{ex}");
        }
    }

}
