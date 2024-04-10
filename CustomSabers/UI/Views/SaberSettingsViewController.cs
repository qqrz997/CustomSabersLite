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
using System.ComponentModel;
using IPA.Config.Data;

namespace CustomSabersLite.UI
{
    [HotReload(RelativePathToLayout = "../BSML/saberSettings.bsml")]
    [ViewDefinition("CustomSabersLite.UI.BSML.saberSettings.bsml")]
    internal class SaberSettingsViewController : BSMLAutomaticViewController, INotifyPropertyChanged, ISharedSaberSettings
    {
        private CSLConfig config;

        [Inject]
        public void Construct(CSLConfig config)
        {
            this.config = config;
        }

        private bool parsed;
        
        [UIComponent("trail-duration")]
        private GenericInteractableSetting trailDurationInteractable;

        [UIComponent("trail-duration")]
        private TextMeshProUGUI trailDurationText;

        [UIComponent("forcefully-foolish")]
        private Transform foolishSetting;

        [UIValue("enabled")]
        public bool Enabled
        {
            get => config.Enabled; 
            set => config.Enabled = value;
        }

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
        public int TrailDuration
        {
            get => config.TrailDuration;
            set => config.TrailDuration = value;
        }

        [UIValue("trail-type")]
        public string trailType
        {
            get => config.TrailType.ToString();
            set => config.TrailType = Enum.TryParse(value, out TrailType trailType) ? trailType : config.TrailType;
        }

        public TrailType TrailType { get; set; } // todo - this is temporary

        [UIValue("trail-type-list")]
        public List<object> trailTypeList = Enum.GetNames(typeof(TrailType)).ToList<object>();

        [UIValue("enable-custom-events")]
        public bool EnableCustomEvents
        {
            get => config.EnableCustomEvents;
            set => config.EnableCustomEvents = value;
        }

        [UIValue("enable-custom-color-scheme")]
        public bool EnableCustomColorScheme
        {
            get => config.EnableCustomColorScheme;
            set => config.EnableCustomColorScheme = value;
        }

        [UIValue("left-saber-color")]
        public Color LeftSaberColor
        {
            get => config.LeftSaberColor;
            set => config.LeftSaberColor = value;
        }

        [UIValue("right-saber-color")]
        public Color RightSaberColor
        {
            get => config.RightSaberColor;
            set => config.RightSaberColor = value;
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
            parsed = true;
            SetTrailDurationInteractable(OverrideTrailDuration);
        }

        public void Activated()
        {
            foreach (string name in SharedProperties.Names)
            {
                NotifyPropertyChanged(name);
            }
            NotifyPropertyChanged(trailType); // todo - this is temporary

            if (config.Fooled)
            {
                foolishSetting.gameObject.SetActive(true);
            }
        }
    }
}
