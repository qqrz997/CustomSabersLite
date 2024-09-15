using UnityEngine;

namespace CustomSabersLite.Models;

internal class DefaultSaberData : ISaberData
{
    public ISaberMetadata Metadata =>
        new CustomSaberMetadata(
            new SaberFileInfo(null, CustomSaberType.Default),
            SaberLoaderError.None,
            new Descriptor("Default", "Beat Games", null),
            new SaberModelFlags(false, []));

    public GameObject GetPrefab(SaberType type) => null;
    public void Dispose() { }
}
