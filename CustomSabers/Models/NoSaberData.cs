using System;

namespace CustomSabersLite.Models;

internal record NoSaberData(string RelativePath, DateTime DateAdded, SaberLoaderError LoaderError) : ISaberData
{
    public ISaberMetadata Metadata => new NoMetadata(RelativePath, DateAdded, LoaderError);

    public static NoSaberData Value { get; } = new NoSaberData(string.Empty, DateTime.MinValue, SaberLoaderError.None);

    public SaberPrefab? Prefab => null;
    public void Dispose() { }
}
