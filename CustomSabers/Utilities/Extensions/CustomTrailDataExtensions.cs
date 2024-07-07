using CustomSabersLite.Data;
using UnityEngine;

namespace CustomSabersLite.Utilities.Extensions;

internal static class CustomTrailDataExtensions
{
    public static Vector3 GetOverrideWidthBottom(this CustomTrailData trailData, int widthPercent)
    {
        var trailTop = trailData.Top.position;
        var trailBottom = trailData.Bottom.position;
        var distance = Vector3.Distance(trailTop, trailBottom);
        var width = distance > 0 ? widthPercent / 100f / distance : 1f;

        return Vector3.LerpUnclamped(trailTop, trailBottom, width);
    }
}
