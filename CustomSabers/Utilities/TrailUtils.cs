using CustomSabersLite.Configuration;
using IPA.Utilities;
using UnityEngine;
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
                    ReflectionUtil.SetField<SaberTrail, Color>(trail, "_color", new Color(0f, 0f, 0f, 0f));
                }
                else
                {
                    trail.enabled = false;
                }
            }
            else
            {
                ReflectionUtil.SetField<SaberTrail, float>(trail, "_trailDuration", trailDuration);
                if (!isDefaultSaber)
                {
                    trail.enabled = true;
                }
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
