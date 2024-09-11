using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using UnityEngine;
using TMPro;
using Zenject;
using System.ComponentModel;
using CustomSabersLite.UI.Managers;
using CustomSabersLite.Utilities;

namespace CustomSabersLite.UI.Views;

[HotReload(RelativePathToLayout = "../BSML/saberSettings.bsml")]
[ViewDefinition("CustomSabersLite.UI.BSML.saberSettings.bsml")]
internal class SaberSettingsViewController : BSMLAutomaticViewController, INotifyPropertyChanged
{
    private CSLConfig config;
    private SaberPreviewManager previewManager;

    [Inject]
    public void Construct(CSLConfig config, SaberPreviewManager previewManager)
    {
        this.config = config;
        this.previewManager = previewManager;
    }

    private bool parsed;
    
    [UIComponent("trail-duration")] private SliderSetting trailDurationSlider;
    [UIComponent("trail-duration")] private TextMeshProUGUI trailDurationText;
    [UIComponent("trail-width")] private SliderSetting trailWidthSlider;
    [UIComponent("trail-width")] private TextMeshProUGUI trailWidthText;
    [UIComponent("forcefully-foolish")] private Transform foolishSetting;

    [UIValue("enabled")]
    private bool Enabled
    {
        get => config.Enabled; 
        set => config.Enabled = value;
    }

    [UIValue("disable-white-trail")]
    private bool DisableWhiteTrail
    {
        get => config.DisableWhiteTrail;
        set
        {
            config.DisableWhiteTrail = value; 
            previewManager.UpdateTrails();
        }
    }

    [UIValue("override-trail-duration")]
    private bool OverrideTrailDuration
    {
        get => config.OverrideTrailDuration;
        set
        {
            config.OverrideTrailDuration = value;
            previewManager.UpdateTrails();
            if (parsed) BSMLHelpers.SetSliderInteractable(trailDurationSlider, value);
        }
    }

    [UIValue("override-trail-width")]
    private bool OverrideTrailWidth
    {
        get => config.OverrideTrailWidth;
        set
        {
            config.OverrideTrailWidth = value;
            previewManager.UpdateTrails();
            if (parsed) BSMLHelpers.SetSliderInteractable(trailWidthSlider, value);
        }
    }

    [UIValue("trail-duration")]
    private int TrailDuration
    {
        get => config.TrailDuration;
        set
        {
            config.TrailDuration = value;
            previewManager.UpdateTrails();
        }
    }

    [UIValue("trail-width")]
    private int TrailWidth
    {
        get => config.TrailWidth;
        set
        {
            config.TrailWidth = value;
            previewManager.UpdateTrails();
        }
    }

    [UIValue("trail-type-choices")] private List<object> trailTypeChoices = Enum.GetNames(typeof(TrailType)).ToList<object>();
    [UIValue("trail-type")]
    private string TrailType
    {
        get => config.TrailType.ToString();
        set
        {
            config.TrailType = Enum.TryParse(value, out TrailType trailType) ? trailType : config.TrailType;
            previewManager.UpdateTrails();
        }
    }

    [UIValue("enable-custom-events")]
    private bool EnableCustomEvents
    {
        get => config.EnableCustomEvents;
        set => config.EnableCustomEvents = value;
    }

    [UIValue("forcefully-foolish")]
    private bool ForcefullyFoolish
    {
        get => config.ForcefullyFoolish;
        set => config.ForcefullyFoolish = value;
    }

    [UIAction("#post-parse")]
    private void PostParse()
    {
        parsed = true;
        BSMLHelpers.SetSliderInteractable(trailDurationSlider, OverrideTrailDuration);
        BSMLHelpers.SetSliderInteractable(trailWidthSlider, OverrideTrailWidth);
    }

    public void Activated()
    {
        SharedSaberSettings.PropertyNames.ForEach(NotifyPropertyChanged);
        foolishSetting.gameObject.SetActive(config.Fooled);
    }
}
