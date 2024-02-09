using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMUI;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomSaber.Configuration;
using CustomSaber.Data;
using CustomSaber.Utilities;

namespace CustomSaber.UI
{
    internal class SaberSettingsViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CustomSaber.UI.Views.saberSettings.bsml";

        public static SaberSettingsViewController Instance;

        [UIValue("disable-white-trail")]
        public bool DisableWhiteTrail
        {
            get => CustomSaberConfig.Instance.DisableWhiteTrail;
            set => CustomSaberConfig.Instance.DisableWhiteTrail = value;
        }

        [UIValue("override-trail-duration")]
        public bool OverrideTrailDuration
        {
            get => CustomSaberConfig.Instance.OverrideTrailDuration;
            set => CustomSaberConfig.Instance.OverrideTrailDuration = value;
        }

        [UIValue("trail-duration")]
        public int TrailDurationMultiplier
        {
            get => CustomSaberConfig.Instance.TrailDuration;
            set => CustomSaberConfig.Instance.TrailDuration = value;
        }

        [UIValue("trail-type")]
        public string TrailType
        {
            get => CustomSaberConfig.Instance.TrailType.ToString();
            set => CustomSaberConfig.Instance.TrailType = Enum.TryParse(value, out TrailType trailType) ? trailType : CustomSaberConfig.Instance.TrailType;
        }

        [UIValue("trail-type-list")]
        public List<object> trailType = Enum.GetNames(typeof(TrailType)).ToList<object>();
    }
}
