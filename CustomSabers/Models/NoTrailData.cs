using UnityEngine;

namespace CustomSabersLite.Models;

internal class NoTrailData : ITrailData
{
    public NoTrailData() { }

    public Material? Material => null;
    public float LengthSeconds => 0f;
    public bool UseCustomColor => false;
    public Color CustomColor => default;
    public Color ColorMultiplier => default;
    public Vector3 TrailTopOffset => default;
    public Vector3 TrailBottomOffset => default;
}