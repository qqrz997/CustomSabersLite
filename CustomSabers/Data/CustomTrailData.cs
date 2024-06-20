using CustomSabersLite.Utilities;
using UnityEngine;

namespace CustomSabersLite.Data;

/// <summary>
/// Class that declares the neccessary information to create a <see cref="Components.Game.LiteSaberTrail"/>
/// </summary>
internal class CustomTrailData(Transform trailTop, Transform trailBottom, Material trailMaterial, Color trailColor, int length = TrailUtils.LegacyDuration)
{
    // Probably should attach an array of this to CustomSaberData and handle it when the saber is loaded so that we know preemptively if a saber has trails, or even has secondary trails

    public Transform TrailTop { get; } = trailTop;
    public Transform TrailBottom { get; } = trailBottom;
    public Material Material { get; } = trailMaterial;
    public Color Color { get; } = trailColor;
    // public Color TrailColorMultiplier { get; }  todo ^
    public int Length { get; } = length;
}
