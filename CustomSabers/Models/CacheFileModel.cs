namespace CustomSabersLite.Models;

internal record CacheFileModel(string Version, SaberMetadataModel[] CachedMetadata)
{
    public static CacheFileModel Empty => new(Plugin.Version.ToString(), []);
}
