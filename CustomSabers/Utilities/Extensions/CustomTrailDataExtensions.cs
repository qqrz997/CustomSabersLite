using CustomSabersLite.Data;
using UnityEngine;

namespace CustomSabersLite.Utilities.Extensions;

internal static class CustomTrailDataExtensions
{
    public static Vector3 GetOverrideWidthBottom(this CustomTrailData trailData, int widthPercent, bool relativePosition = false) => 
        trailData.Bottom && trailData.Top ? CalculateBottom(trailData, widthPercent, relativePosition)
        : Vector3.zero;

    private static Vector3 CalculateBottom(CustomTrailData trailData, int widthPercent, bool relativePosition)
    {
        var top = relativePosition ? trailData.Top.localPosition : trailData.Top.position;
        var bottom = relativePosition ? trailData.Bottom.localPosition : trailData.Bottom.position;
        var distance = Vector3.Distance(top, bottom);
        var width = distance > 0 ? widthPercent / 100f / distance : 1f;

        return Vector3.LerpUnclamped(top, bottom, width);
    }
}
