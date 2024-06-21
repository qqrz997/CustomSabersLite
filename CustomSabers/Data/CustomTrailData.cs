using UnityEngine;

namespace CustomSabersLite.Data;

/// <summary>
/// Class that declares the neccessary information to create a <see cref="Components.Game.LiteSaberTrail"/>
/// </summary>
internal readonly record struct CustomTrailData(Transform Top, Transform Bottom, Material Material, CustomSaber.ColorType ColorType, Color Color, Color ColorMultiplier, float Length);
