using System;
using System.IO;

namespace CustomSabersLite.Models;

internal record NoSaberData(string FullPath, DateTime DateAdded, SaberLoaderError LoaderError) : ISaberData
{
    public ISaberMetadata Metadata => new NoMetadata(Path.GetFileName(FullPath), DateAdded, LoaderError);

    public static NoSaberData Value { get; } = new(string.Empty, DateTime.MinValue, SaberLoaderError.None);

    public SaberPrefab? Prefab => null;
    public void Dispose() { }
}
