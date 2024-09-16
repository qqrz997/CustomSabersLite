using System;
using UnityEngine;

namespace CustomSabersLite.Models;

internal interface ISaberData : IDisposable
{
    ISaberMetadata Metadata { get; }

    GameObject? GetPrefab(SaberType type);
}
