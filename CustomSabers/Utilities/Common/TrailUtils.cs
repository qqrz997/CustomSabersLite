using System.Collections.Generic;
using CustomSabersLite.Components;
using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities.Extensions;

namespace CustomSabersLite.Utilities.Common;

internal static class TrailUtils
{
    // Legacy trail duration in frames
    public const int LegacyDuration = 30;

    // Default duration of the saber trail in seconds
    public const float DefaultDuration = 0.4f;

    /// <summary>
    /// Converts legacy trail length in frames to trail duration in seconds used by <see cref="SaberTrail"/>
    /// </summary>
    /// <param name="customTrailLength">Trail length from a CustomTrail, in frames</param>
    /// <returns>Trail duration in seconds</returns>
    public static float ConvertLegacyLength(int customTrailLength) =>
        customTrailLength / (float)LegacyDuration * DefaultDuration;

    /// <summary>
    /// Configures a collection of trails. The first trail in the sequence will be enabled for overriding width, any
    /// other trails will not be able to override width.
    /// </summary>
    /// <param name="trails">The trails to configure</param>
    /// <param name="config">The config to use</param>
    public static void ConfigureTrails(this IList<LiteSaberTrail> trails, PluginConfig config)
    {
        for (int i = 0; i < trails.Count; i++)
        {
            var trail = trails[i];
            if (trail != null) trail.ConfigureTrail(config, i == 0);
        }
    }

    /// <summary>
    /// Uses the current config to decide the trail's length, width, white section, and visibility.
    /// </summary>
    /// <param name="trail">The trail being configured</param>
    /// <param name="config">The config to use</param>
    /// <param name="useOverrideWidth">If set to true, then the trail will be able to make use of the width override in
    /// the config</param>
    private static void ConfigureTrail(this LiteSaberTrail trail, PluginConfig config, bool useOverrideWidth = false)
    {
        if (trail._trailRenderer == null)
        {
            return;
        }

        trail._whiteSectionMaxDuration = config.DisableWhiteTrail ? 0f : 0.03f;
        trail._framesPassed = 0;
        trail._framesToScaleCheck = 0;
        trail._inited = false;

        trail.OverrideWidth = config.TrailWidth;
        trail.UseWidthOverride = config.OverrideTrailWidth && useOverrideWidth;

        trail._trailDuration = config.OverrideTrailDuration ? config.TrailDuration * DefaultDuration
            : trail.TrailData.LengthSeconds;

        trail.enabled = !trail._trailDuration.Approximately(0f);
    }
}
