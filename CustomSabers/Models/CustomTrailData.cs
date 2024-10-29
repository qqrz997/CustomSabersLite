using UnityEngine;
using TrailColorType = CustomSaber.ColorType;

namespace CustomSabersLite.Models;

/// <summary>
/// Class that declares the neccessary information to create a <see cref="Components.LiteSaberTrail"/>
/// </summary>
internal class CustomTrailData(Transform top, Transform bottom, Material? material, TrailColorType colorType, Color color, Color colorMultiplier, float length)
{
    private readonly Transform top = top;
    private readonly Transform bottom = bottom;

    public Material? Material { get; } = material;

    public TrailColorType ColorType { get; } = colorType;

    public Color Color { get; } = color;

    public Color ColorMultiplier { get; } = colorMultiplier;

    public float Length { get; } = length;

    public Vector3 TopPosition => top.position;

    public Vector3 BottomPosition => bottom.position;

    public Vector3 TopLocalPosition => top.localPosition;

    public Vector3 BottomLocalPosition => bottom.localPosition;
};
