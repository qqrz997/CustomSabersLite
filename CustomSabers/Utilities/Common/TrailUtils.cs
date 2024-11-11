using CustomSabersLite.Components;
using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using UnityEngine;

namespace CustomSabersLite.Utilities;

internal static class TrailUtils
{
    // Legacy trail duration in frames
    public const int LegacyDuration = 30;

    // Default duration of the saber trail in seconds
    public const float DefaultDuration = 0.4f;

    /// <summary>
    /// Converts legacy trail length to trail duration in seconds used by <seealso cref="SaberTrail"/>
    /// </summary>
    public static float ConvertLegacyLength(int customTrailLength) =>
        customTrailLength / (float)LegacyDuration * DefaultDuration;

    /// <summary>
    /// Uses the current <seealso cref="CSLConfig"/> to decide the <seealso cref="SaberTrail"/>'s length, whitestep, and visibility
    /// </summary>
    public static void ConfigureTrail(this SaberTrail trail, CSLConfig config, bool useOverrideWidth = false)
    {
        if (!trail._trailRenderer)
        {
            return;
        }

        trail._whiteSectionMaxDuration = config.DisableWhiteTrail ? 0f : 0.03f;
        trail._framesPassed = 0;
        trail._framesToScaleCheck = 0;
        trail._inited = false;

        if (trail is LiteSaberTrail customTrail && customTrail.InstanceTrailData != null)
        {
            float duration = config.OverrideTrailDuration ? config.TrailDuration / 250f
                : customTrail.InstanceTrailData.Length;
            customTrail.OverrideWidth = config.TrailWidth;
            customTrail.UseWidthOverride = config.OverrideTrailWidth && useOverrideWidth;
            customTrail._trailDuration = duration;
            customTrail.enabled = config.TrailType != TrailType.None && !Mathf.Approximately(duration, 0f);
        }
        else if (trail is SaberTrail defaultTrail)
        {
            float duration = config.OverrideTrailDuration ? config.TrailDuration / 250f : DefaultDuration;
            defaultTrail._trailDuration = duration;
            if (config.TrailType == TrailType.None || Mathf.Approximately(duration, 0f))
            {
                defaultTrail._color.a = 0f;
            }
        }
    }
}
