using UnityEngine;
using TrailColorType = CustomSaber.ColorType;

namespace CustomSabersLite.Models;

/// <summary>
/// Class that declares the necessary information to create a <see cref="Components.LiteSaberTrail"/>
/// </summary>
internal class CustomTrailData(Transform top, Transform bottom, Material? material, TrailColorType colorType, Color color, Color colorMultiplier, float length)
{
    private readonly Transform top = top;
    private readonly Transform bottom = bottom;
    
    private readonly TrailColorType colorType = colorType;
    private readonly Color color = color;
    private readonly Color colorMultiplier = colorMultiplier;

    public Material? Material { get; } = material;
    public float Length { get; } = length;

    public Vector3 TopPosition => top.position;
    public Vector3 BottomPosition => bottom.position;
    public Vector3 TopLocalPosition => top.localPosition;
    public Vector3 BottomLocalPosition => bottom.localPosition;
    
    public Vector3 GetOverrideWidthBottom(float trailWidth)
    {
        float distance = Vector3.Distance(TopPosition, BottomPosition);
        float width = distance > 0 ? trailWidth / distance : 1f;
        return Vector3.LerpUnclamped(TopPosition, BottomPosition, width);
    }

    public Color GetTrailColor(Color baseColor) => 
        colorMultiplier * (colorType == TrailColorType.CustomColor ? color : baseColor);

    public Color GetTrailColor() => colorMultiplier * color;
};
