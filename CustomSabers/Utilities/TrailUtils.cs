using CustomSabersLite.Configuration;
using CustomSabersLite.Data;

namespace CustomSabersLite.Utilities
{
    internal static class TrailUtils
    {
        // Default duration of the saber trail - measured in seconds
        private const float DefaultDuration = 0.4f;

        /// <summary>
        /// Uses the current <seealso cref="CSLConfig"/> to decide the <seealso cref="SaberTrail"/>'s length, whitestep, and visibility
        /// </summary>
        public static void ConfigureTrail(this SaberTrail trail, CSLConfig config)
        {
            if (config.TrailType == TrailType.None)
            {
                trail.Disable();
            }
            else
            {
                if (config.OverrideTrailDuration)
                {
                    trail.SetTrailDuration(config.TrailDuration / 100f * DefaultDuration);
                }
                if (config.DisableWhiteTrail)
                {
                    trail.DisableWhiteTrail();
                }
            }
        }

        private static void SetTrailDuration(this SaberTrail trail, float trailDuration)
        {
            trail._trailDuration = trailDuration;

            if (trailDuration.Equals(0f))
            {
                trail.Disable();
            }
            else
            {
                trail.Enable();
            }
        }

        private static void Enable(this SaberTrail trail)
        {
            // Only disable/enable trails that inherit from SaberTrail
            // If we disable/enable the default trail, there can be compatibility issues with other mods
            // Instead, just make the default trail transparent
            if (!trail.IsDefault())
            {
                trail.enabled = true;
            }
            else if (trail._color.a.Equals(0f))
            {
                trail._color.a = 1f; // todo - test if the default trail color gets reset automatically
            }
        }

        private static void Disable(this SaberTrail trail)
        {
            if (!trail.IsDefault())
            {
                trail.enabled = false;
            }
            else
            {
                trail._color.a = 0f;
            }
        }

        private static bool IsDefault(this SaberTrail trail) =>
            trail.GetType() == typeof(SaberTrail);

        private static void DisableWhiteTrail(this SaberTrail trail) =>
            trail._whiteSectionMaxDuration = 0f;
    }
}
