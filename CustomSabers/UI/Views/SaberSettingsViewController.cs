using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.TypeHandlers;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using UnityEngine;
using TMPro;

namespace CustomSabersLite.UI
{
    internal class SaberSettingsViewController : BSMLResourceViewController
    {
        public override string ResourceName => "CustomSabersLite.UI.BSML.saberSettings.bsml";

        public static SaberSettingsViewController Instance;

        private bool parsed;
        
        [UIComponent("trail-duration")]
        private GenericInteractableSetting trailDurationInteractable;

        [UIComponent("trail-duration")]
        private TextMeshProUGUI trailDurationText;

        [UIValue("disable-white-trail")]
        public bool DisableWhiteTrail
        {
            get => CSLConfig.Instance.DisableWhiteTrail;
            set => CSLConfig.Instance.DisableWhiteTrail = value;
        }

        [UIValue("override-trail-duration")]
        public bool OverrideTrailDuration
        {
            get => CSLConfig.Instance.OverrideTrailDuration;
            set
            {
                CSLConfig.Instance.OverrideTrailDuration = value;
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
            get => CSLConfig.Instance.TrailDuration;
            set => CSLConfig.Instance.TrailDuration = value;
        }

        [UIValue("trail-type")]
        public string TrailType
        {
            get => CSLConfig.Instance.TrailType.ToString();
            set => CSLConfig.Instance.TrailType = Enum.TryParse(value, out TrailType trailType) ? trailType : CSLConfig.Instance.TrailType;
        }

        [UIValue("trail-type-list")]
        public List<object> trailType = Enum.GetNames(typeof(TrailType)).ToList<object>();

        [UIValue("enable-custom-events")]
        public bool CustomEventsEnabled
        {
            get => CSLConfig.Instance.CustomEventsEnabled;
            set => CSLConfig.Instance.CustomEventsEnabled = value;
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            parsed = true;
            SetTrailDurationInteractable(OverrideTrailDuration);
        }
    }
}
