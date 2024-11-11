using CustomSabersLite.Models;
using UnityEngine;

namespace CustomSabersLite.Utilities;

internal static class CustomTrailDataExtensions
{
    public static Vector3 GetOverrideWidthBottom(this CustomTrailData trailData, int widthPercent, bool relativePosition = false)
    {
        var top = relativePosition ? trailData.TopLocalPosition : trailData.TopPosition;
        var bottom = relativePosition ? trailData.BottomLocalPosition : trailData.BottomPosition;
        float distance = Vector3.Distance(top, bottom);
        float width = distance > 0 ? widthPercent / 100f / distance : 1f;
        var overrideBottom = Vector3.LerpUnclamped(top, bottom, width);
        return overrideBottom;
    }
}
