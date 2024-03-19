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
using Zenject;

namespace CustomSabersLite.UI
{
    internal class SaberSettingsViewController : BSMLResourceViewController
    {
        private CSLConfig config;

        [Inject]
        public void Construct(CSLConfig config)
        {
            this.config = config;
        }

        public override string ResourceName => "CustomSabersLite.UI.BSML.saberSettings.bsml";

        public static SaberSettingsViewController Instance;

        private bool parsed;
        
        [UIComponent("trail-duration")]
        private GenericInteractableSetting trailDurationInteractable;

        [UIComponent("trail-duration")]
        private TextMeshProUGUI trailDurationText;

        [UIComponent("forcefully-foolish")]
        private Transform foolishSetting;

        [UIValue("disable-white-trail")]
        public bool DisableWhiteTrail
        {
            get => config.DisableWhiteTrail;
            set => config.DisableWhiteTrail = value;
        }

        [UIValue("override-trail-duration")]
        public bool OverrideTrailDuration
        {
            get => config.OverrideTrailDuration;
            set
            {
                config.OverrideTrailDuration = value;
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
            get => config.TrailDuration;
            set => config.TrailDuration = value;
        }

        [UIValue("trail-type")]
        public string TrailType
        {
            get => config.TrailType.ToString();
            set => config.TrailType = Enum.TryParse(value, out TrailType trailType) ? trailType : config.TrailType;
        }

        [UIValue("trail-type-list")]
        public List<object> trailType = Enum.GetNames(typeof(TrailType)).ToList<object>();

        [UIValue("enable-custom-events")]
        public bool CustomEventsEnabled
        {
            get => config.CustomEventsEnabled;
            set => config.CustomEventsEnabled = value;
        }

        [UIValue("forcefully-foolish")]
        public bool ForcefullyFoolish
        {
            get => config.ForcefullyFoolish;
            set => config.ForcefullyFoolish = value;
        }

        [UIAction("#post-parse")]
        private void PostParse()
        {
            if (config.Fooled)
            {
                foolishSetting.gameObject.SetActive(true);
            }

            parsed = true;
            SetTrailDurationInteractable(OverrideTrailDuration);
        }
    }
}
