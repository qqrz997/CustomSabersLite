using System;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class NoSaberData(string relativePath, SaberLoaderError loaderError) : ISaberData
{
    public ISaberMetadata Metadata => new NoMetadata(relativePath, loaderError);

    public GameObject GetPrefab(SaberType type) => null;
    public void Dispose() { }
}
