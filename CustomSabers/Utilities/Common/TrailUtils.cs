using CustomSabersLite.Components;
using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Extensions;

namespace CustomSabersLite.Utilities.Common;

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
    /// Uses the current <seealso cref="CslConfig"/> to decide the <seealso cref="SaberTrail"/>'s length, whitestep, and visibility
    /// </summary>
    public static void ConfigureTrail(this SaberTrail trail, CslConfig config, bool useOverrideWidth = false)
    {
        if (trail == null || trail._trailRenderer == null)
        {
            return;
        }

        trail._whiteSectionMaxDuration = config.DisableWhiteTrail ? 0f : 0.03f;
        trail._framesPassed = 0;
        trail._framesToScaleCheck = 0;
        trail._inited = false;

        if (trail is LiteSaberTrail { InstanceTrailData: not null } customTrail)
        {
            float duration = config.OverrideTrailDuration ? config.TrailDuration * DefaultDuration
                : customTrail.InstanceTrailData.Length;
            customTrail.OverrideWidth = config.TrailWidth;
            customTrail.UseWidthOverride = config.OverrideTrailWidth && useOverrideWidth;
            customTrail._trailDuration = duration;
            customTrail.enabled = config.TrailType != TrailType.None && !duration.Approximately(0f);
        }
        else
        {
            float duration = config.OverrideTrailDuration ? config.TrailDuration * DefaultDuration : DefaultDuration;
            trail._trailDuration = duration;
            if (config.TrailType == TrailType.None || duration.Approximately(0f))
            {
                trail._color.a = 0f;
            }
        }
    }
}
