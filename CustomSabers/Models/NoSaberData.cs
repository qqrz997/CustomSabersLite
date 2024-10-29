using System;
using UnityEngine;

namespace CustomSabersLite.Models;

internal record NoSaberData(string FullPath, DateTime DateAdded, SaberLoaderError LoaderError) : ISaberData
{
    public ISaberMetadata Metadata => new NoMetadata(FullPath, DateAdded, LoaderError);

    public static NoSaberData Value { get; } = new NoSaberData(string.Empty, DateTime.MinValue, SaberLoaderError.None);

    public SaberPrefab? Prefab => null;
    public GameObject? GetPrefab(SaberType saberType) => null;
    public void Dispose() { }
}
