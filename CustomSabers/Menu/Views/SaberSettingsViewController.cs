using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities.Extensions;
using Zenject;

namespace CustomSabersLite.Menu.Views;

[HotReload(RelativePathToLayout = "../BSML/saberSettings.bsml")]
[ViewDefinition("CustomSabersLite.Menu.BSML.saberSettings.bsml")]
internal class SaberSettingsViewController : BSMLAutomaticViewController, ISharedSaberSettings
{
    [Inject] private readonly PluginConfig config = null!;
    [Inject] private readonly SaberPreviewManager previewManager = null!;
    
    public bool Enabled
    {
        get => config.Enabled;
        set => config.Enabled = value;
    }

    public bool DisableWhiteTrail
    {
        get => config.DisableWhiteTrail;
        set
        {
            config.DisableWhiteTrail = value;
            previewManager.UpdateTrails();
        }
    }

    public bool OverrideTrailDuration
    {
        get => config.OverrideTrailDuration;
        set
        {
            config.OverrideTrailDuration = value;
            previewManager.UpdateTrails();
        }
    }

    public bool OverrideTrailWidth
    {
        get => config.OverrideTrailWidth;
        set
        {
            config.OverrideTrailWidth = value;
            previewManager.UpdateTrails();
        }
    }

    public float TrailDuration
    {
        get => config.TrailDuration;
        set
        {
            config.TrailDuration = value;
            previewManager.UpdateTrails();
        }
    }

    public float TrailWidth
    {
        get => config.TrailWidth;
        set
        {
            config.TrailWidth = value;
            previewManager.UpdateTrails();
        }
    }
    
    public bool OverrideSaberLength
    {
        get => config.OverrideSaberLength;
        set
        {
            config.OverrideSaberLength = value;
            previewManager.UpdateSaberModels();
        }
    }
    
    public float SaberLength
    {
        get => config.SaberLength;
        set
        {
            config.SaberLength = value;
            previewManager.UpdateSaberModels();
        }
    }
    
    public bool OverrideSaberWidth
    {
        get => config.OverrideSaberWidth;
        set
        {
            config.OverrideSaberWidth = value;
            previewManager.UpdateSaberModels();
        }
    }
    
    public float SaberWidth
    {
        get => config.SaberWidth;
        set
        {
            config.SaberWidth = value;
            previewManager.UpdateSaberModels();
        }
    }

    public bool EnableCustomEvents
    {
        get => config.EnableCustomEvents;
        set => config.EnableCustomEvents = value;
    }
    
    protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
    {
        base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
        ISharedSaberSettings.PropertyNames.ForEach(NotifyPropertyChanged);
    }
}
