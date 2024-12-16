using System;

namespace CustomSabersLite.Models;

internal interface ISaberData : IDisposable
{
    ISaberMetadata Metadata { get; }

    SaberPrefab? Prefab { get; }
}
