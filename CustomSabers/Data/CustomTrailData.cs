using UnityEngine;

namespace CustomSabersLite.Data;

/// <summary>
/// Class that declares the neccessary information to create a <see cref="Components.Game.LiteSaberTrail"/>
/// </summary>
internal readonly struct CustomTrailData(Transform top, Transform bottom, Material material, CustomSaber.ColorType colorType, Color color, Color colorMultiplier, float length)
{
    public Transform Top { get; } = top;

    public Transform Bottom { get; } = bottom;

    public Material Material { get; } = material;

    public CustomSaber.ColorType ColorType { get; } = colorType;

    public Color Color { get; } = color;

    public Color ColorMultiplier { get; } = colorMultiplier;

    public float Length { get; } = length;
};
