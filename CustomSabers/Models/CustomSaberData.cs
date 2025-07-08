namespace CustomSabersLite.Models;

internal class CustomSaberData(
    CustomSaberMetadata metadata,
    ISaberPrefab customSaberPrefab) : ISaberData
{
    public CustomSaberMetadata Metadata { get; } = metadata;
    public ISaberPrefab Prefab { get; } = customSaberPrefab;

    public void Dispose()
    {
        Prefab.Dispose();
    }
}