using System;

namespace CustomSabersLite.Models;

internal interface ISaberData : IDisposable
{
    CustomSaberMetadata? Metadata { get; }

    ISaberPrefab? Prefab { get; }
}
