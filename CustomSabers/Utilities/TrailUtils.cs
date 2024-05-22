using CustomSabersLite.Configuration;
using UnityEngine;

namespace CustomSabersLite.Utilities
{
    internal class TrailUtils
    {
        private readonly CSLConfig config;

        public TrailUtils(CSLConfig config)
        {
            this.config = config;
        }

        public void SetTrailDuration(SaberTrail trail, bool isDefaultSaber = false, float trailDuration = 0.4f)
        {
            if (config.OverrideTrailDuration)
            {
                trailDuration = config.TrailDuration / 100f * trailDuration;
            }

            if (trailDuration.Equals(0f))
            {
                if (isDefaultSaber)
                {
                    trail._color = Color.clear;
                }
                else
                {
                    trail.enabled = false;
                }
            }
            else
            {
                trail._trailDuration = trailDuration;
                if (!isDefaultSaber)
                {
                    trail.enabled = true;
                }
            }
        }

        public void SetWhiteTrailDuration(SaberTrail trail, float whiteSectionMaxDuration = 0.03f) => 
            trail._whiteSectionMaxDuration = config.DisableWhiteTrail ? 0f : whiteSectionMaxDuration;
    }
}
