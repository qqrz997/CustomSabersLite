﻿using System;
using System.Collections.Generic;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using Zenject;
using CustomSabersLite.Utilities;
using HMUI;

namespace CustomSabersLite.UI.Views;

[HotReload(RelativePathToLayout = "../BSML/saberSettings.bsml")]
[ViewDefinition("CustomSabersLite.UI.BSML.saberSettings.bsml")]
internal class SaberSettingsViewController : BSMLAutomaticViewController
{
    [Inject] private readonly CSLConfig config = null!;
    [Inject] private readonly SaberPreviewManager previewManager = null!;

    [UIComponent("trail-duration")] private readonly ImageView trailDurationIcon = null!; 
    [UIComponent("trail-width")] private readonly ImageView trailWidthIcon = null!;
    [UIComponent("saber-length")] private readonly ImageView saberLengthIcon = null!;
    [UIComponent("saber-width")] private readonly ImageView saberWidthIcon = null!;

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
    
    [UIValue("override-saber-length")]
    private bool OverrideSaberLength
    {
        get => config.OverrideSaberLength;
        set
        {
            config.OverrideSaberLength = value;
            previewManager.UpdateSaberModels();
        }
    }
    
    [UIValue("saber-length")]
    private int SaberLength
    {
        get => config.SaberLength;
        set
        {
            config.SaberLength = value;
            previewManager.UpdateSaberModels();
        }
    }
    
    [UIValue("override-saber-width")]
    private bool OverrideSaberWidth
    {
        get => config.OverrideSaberWidth;
        set
        {
            config.OverrideSaberWidth = value;
            previewManager.UpdateSaberModels();
        }
    }
    
    [UIValue("saber-width")]
    private int SaberWidth
    {
        get => config.SaberWidth;
        set
        {
            config.SaberWidth = value;
            previewManager.UpdateSaberModels();
        }
    }
    
    [UIValue("trail-type-choices")] private List<object> trailTypeChoices = [.. Enum.GetNames(typeof(TrailType))];
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

    [UIAction("#post-parse")]
    private void PostParse()
    {
        trailDurationIcon.sprite = CSLResources.TrailDurationIcon;
        trailWidthIcon.sprite = CSLResources.TrailWidthIcon;
        saberLengthIcon.sprite = CSLResources.SaberLengthIcon;
        saberWidthIcon.sprite = CSLResources.SaberWidthIcon;
    }
    
    [UIAction("percent-slider-formatter")]
    private string PercentSliderFormatter(object value) => $"{value}%";

    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
        SharedSaberSettings.PropertyNames.ForEach(NotifyPropertyChanged);
    }
}
