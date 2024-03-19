using CustomSabersLite.Configuration;
using IPA.Utilities;
using Zenject;

namespace CustomSabersLite.Utilities
{
    internal class TrailUtils
    {
        CSLConfig config;

        public TrailUtils(CSLConfig config)
        {
            this.config = config;
        }

        public void SetTrailDuration(SaberTrail trail, float trailDuration = 0.4f)
        {
            if (config.OverrideTrailDuration)
            {
                trailDuration = config.TrailDuration / 100f * trailDuration;
            }

            if (trailDuration == 0)
            {
                trail.enabled = false;
            }
            else
            {
                ReflectionUtil.SetField<SaberTrail, float>(trail, "_trailDuration", trailDuration);
                trail.enabled = true;
            }
        }

        public void SetWhiteTrailDuration(SaberTrail trail, float whiteSectionMaxDuration = 0.03f)
        {
            if (config.DisableWhiteTrail)
            {
                whiteSectionMaxDuration = 0f;
            }
            ReflectionUtil.SetField<SaberTrail, float>(trail, "_whiteSectionMaxDuration", whiteSectionMaxDuration);
        }
    }
}
