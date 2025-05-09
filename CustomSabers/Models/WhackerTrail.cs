using Newtonsoft.Json;
using UnityEngine;

namespace CustomSabersLite.Models;

internal class WhackerTrail
{
    public int TrailId { get; }
    public CustomSaber.ColorType ColorType { get; }
    public Color TrailColor { get; }
    public Color MultiplierColor { get; }
    public int Length { get; }

    [JsonConstructor]
    public WhackerTrail(
        int trailId,
        CustomSaber.ColorType colorType,
        Color trailColor,
        Color multiplierColor,
        int length)
    {
        TrailId = trailId;
        ColorType = colorType;
        TrailColor = trailColor;
        MultiplierColor = multiplierColor;
        Length = length;
    }
}
