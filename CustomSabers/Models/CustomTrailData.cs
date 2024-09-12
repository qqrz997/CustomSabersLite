using CustomSabersLite.Utilities;
using UnityEngine;

namespace CustomSabersLite.Models;

/// <summary>
/// Class that declares the neccessary information to create a <see cref="Components.Game.LiteSaberTrail"/>
/// </summary>
internal class CustomTrailData(Transform top, Transform bottom, Material material, CustomSaber.ColorType colorType, Color color, Color colorMultiplier, float length)
{
    private readonly Transform top = top;
    private readonly Transform bottom = bottom;

    public Material Material { get; } = material;

    public CustomSaber.ColorType ColorType { get; } = colorType;

    public Color Color { get; } = color;

    public Color ColorMultiplier { get; } = colorMultiplier;

    public float Length { get; } = length;

    public Vector3 TopPosition => top.position;

    public Vector3 BottomPosition => bottom.position;

    public Vector3 TopLocalPosition => top.localPosition;

    public Vector3 BottomLocalPosition => bottom.localPosition;

    public static CustomTrailData Default => new(
        new GameObject().transform,
        new GameObject().transform,
        null,
        CustomSaber.ColorType.CustomColor,
        Color.white,
        Color.white,
        TrailUtils.DefaultDuration
    );
};
