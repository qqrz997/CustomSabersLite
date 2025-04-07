using UnityEngine;

namespace CustomSabersLite.Models;

internal class CustomSaberData(
    CustomSaberMetadata metadata,
    AssetBundle assetBundle,
    ISaberPrefab customSaberPrefab) : ISaberData
{
    public CustomSaberMetadata Metadata { get; } = metadata;
    public ISaberPrefab Prefab { get; } = customSaberPrefab;
    private AssetBundle Bundle { get; } = assetBundle;

    public void Dispose()
    {
        Prefab.Dispose();
        if (Bundle != null) Bundle.Unload(true);
    }
}