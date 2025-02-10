using System;

namespace CustomSabersLite.Models;

internal record NoSaberData(string FullPath, DateTime DateAdded, SaberLoaderError LoaderError) : ISaberData
{
    // todo: remove this
    public static NoSaberData Value { get; } = new(string.Empty, DateTime.MinValue, SaberLoaderError.None);
    
    public CustomSaberMetadata? Metadata => null;
    public ISaberPrefab? Prefab => null;
    public void Dispose() { }
}
