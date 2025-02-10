using System;

namespace CustomSabersLite.Models;

internal interface ISaberData : IDisposable
{
    public CustomSaberMetadata? Metadata { get; }
    public ISaberPrefab? Prefab { get; }
}