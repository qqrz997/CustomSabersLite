using System;
using UnityEngine;

namespace CustomSabersLite.Models;

internal interface ISaberData : IDisposable
{
    ISaberMetadata Metadata { get; }

    SaberPrefab? Prefab { get; }

    GameObject? GetPrefab(SaberType saberType);
}
