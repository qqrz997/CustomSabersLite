using Newtonsoft.Json.Serialization;

namespace CustomSabersLite.Models;

public class CacheFileModel(string version, SaberMetadataModel[] cachedMetadata)
{
    public string Version { get; } = version;

    public SaberMetadataModel[] CachedMetadata { get; } = cachedMetadata;
}
