using System;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class NoSaberData() : ISaberData
{
    private readonly string relativePath = string.Empty;
    private readonly SaberLoaderError loaderError = SaberLoaderError.None;

    public NoSaberData(string relativePath, SaberLoaderError loaderError) : this()
    {
        this.relativePath = relativePath;
        this.loaderError = loaderError;
    }

    public ISaberMetadata Metadata => new NoMetadata(relativePath, loaderError);

    public GameObject? GetPrefab(SaberType type) => null;
    public void Dispose() { }
}
