using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.TypeHandlers;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomSaber.Configuration;
using CustomSaber.Data;
using UnityEngine;
using TMPro;

namespace CustomSaber.UI
{
    internal class SaberSettingsViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CustomSaber.UI.BSML.saberSettings.bsml";

        public static SaberSettingsViewController Instance;

        private bool parsed;
        
        [UIComponent("trail-duration")]
        private GenericInteractableSetting trailDurationInteractable;

        [UIComponent("trail-duration")]
        private TextMeshProUGUI trailDurationText;

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
            set
            {
                CustomSaberConfig.Instance.OverrideTrailDuration = value;
                SetTrailDurationInteractable(value);
            }
        }

        private void SetTrailDurationInteractable(bool value)
        {
            if (parsed)
            {
                trailDurationText.color = new Color(1f, 1f, 1f, value ? 1f : 0.5f);
                trailDurationInteractable.interactable = OverrideTrailDuration;
            }
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

        [UIValue("enable-custom-events")]
        public bool CustomEventsEnabled
        {
            get => CustomSaberConfig.Instance.CustomEventsEnabled;
            set => CustomSaberConfig.Instance.CustomEventsEnabled = value;
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            parsed = true;
            SetTrailDurationInteractable(OverrideTrailDuration);
        }
    }
}
