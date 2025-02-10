using UnityEngine;
using TrailColorType = CustomSaber.ColorType;

namespace CustomSabersLite.Models;

internal class CustomTrailData : ITrailData
{
    public CustomTrailData(
        Material? material,
        float lengthSeconds,
        TrailColorType colorType,
        Color customColor,
        Color colorMultiplier,
        Vector3 trailTopOffset,
        Vector3 trailBottomOffset)
    {
        Material = material;
        LengthSeconds = lengthSeconds;
        UseCustomColor = colorType == TrailColorType.CustomColor;
        CustomColor = customColor;
        ColorMultiplier = colorMultiplier;
        TrailTopOffset = trailTopOffset;
        TrailBottomOffset = trailBottomOffset;
    }

    public CustomTrailData(
        Material? material,
        float lengthSeconds,
        TrailColorType colorType,
        Color customColor,
        Color colorMultiplier,
        GameObject saberObjectRoot,
        Transform trailTop,
        Transform trailBottom)
    {
        Material = material;
        LengthSeconds = lengthSeconds;
        UseCustomColor = colorType == TrailColorType.CustomColor;
        CustomColor = customColor;
        ColorMultiplier = colorMultiplier;
        TrailTopOffset = trailTop.position - saberObjectRoot.transform.position;
        TrailBottomOffset = trailBottom.position - saberObjectRoot.transform.position;
    }

    public Material? Material { get; }
    public float LengthSeconds { get; }
    public bool UseCustomColor { get; }
    public Color CustomColor { get; }
    public Color ColorMultiplier { get; }
    public Vector3 TrailTopOffset { get; }
    public Vector3 TrailBottomOffset { get; }
}
