using System;
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
internal class SaberSettingsViewController : BSMLAutomaticViewController, ISharedSaberSettings
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
    public bool DisableWhiteTrail
    {
        get => config.DisableWhiteTrail;
        set
        {
            config.DisableWhiteTrail = value;
            previewManager.UpdateTrails();
        }
    }

    [UIValue("override-trail-duration")]
    public bool OverrideTrailDuration
    {
        get => config.OverrideTrailDuration;
        set
        {
            config.OverrideTrailDuration = value;
            previewManager.UpdateTrails();
        }
    }

    [UIValue("override-trail-width")]
    public bool OverrideTrailWidth
    {
        get => config.OverrideTrailWidth;
        set
        {
            config.OverrideTrailWidth = value;
            previewManager.UpdateTrails();
        }
    }

    [UIValue("trail-duration")]
    public float TrailDuration
    {
        get => config.TrailDuration;
        set
        {
            config.TrailDuration = value;
            previewManager.UpdateTrails();
        }
    }

    [UIValue("trail-width")]
    public float TrailWidth
    {
        get => config.TrailWidth;
        set
        {
            config.TrailWidth = value;
            previewManager.UpdateTrails();
        }
    }
    
    [UIValue("override-saber-length")]
    public bool OverrideSaberLength
    {
        get => config.OverrideSaberLength;
        set
        {
            config.OverrideSaberLength = value;
            previewManager.UpdateSaberModels();
        }
    }
    
    [UIValue("saber-length")]
    public float SaberLength
    {
        get => config.SaberLength;
        set
        {
            config.SaberLength = value;
            previewManager.UpdateSaberModels();
        }
    }
    
    [UIValue("override-saber-width")]
    public bool OverrideSaberWidth
    {
        get => config.OverrideSaberWidth;
        set
        {
            config.OverrideSaberWidth = value;
            previewManager.UpdateSaberModels();
        }
    }
    
    [UIValue("saber-width")]
    public float SaberWidth
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
    public string TrailType
    {
        get => config.TrailType.ToString();
        set
        {
            config.TrailType = Enum.TryParse(value, out TrailType trailType) ? trailType : config.TrailType;
            previewManager.UpdateTrails();
        }
    }

    [UIValue("enable-custom-events")]
    public bool EnableCustomEvents
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
    
    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
        ISharedSaberSettings.PropertyNames.ForEach(NotifyPropertyChanged);
    }
}
