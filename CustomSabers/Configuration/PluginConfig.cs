using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Extensions;

namespace CustomSabersLite.Configuration;

internal class PluginConfig
{
    public bool Enabled { get; set; }

    public SaberValue CurrentlySelectedSaber { get; set; }
    public SaberValue CurrentlySelectedTrail { get; set; }
    
    public bool DisableWhiteTrail { get; set; }
    
    public bool OverrideTrailDuration { get; set; }
    public float TrailDuration { get; set; }

    public bool OverrideTrailWidth { get; set; }
    public float TrailWidth { get; set; }

    public bool OverrideSaberLength { get; set; }
    public float SaberLength { get; set; }
    
    public bool OverrideSaberWidth { get; set; }
    public float SaberWidth { get; set; }
        
    public bool EnableCustomEvents { get; set; }

    // Don't serialize these
    public bool EnableMenuSabers { get; set; } = false;
    public OrderBy OrderByFilter { get; set; } = OrderBy.Name;
    public string SearchFilter { get; set; } = string.Empty;
    public bool ReverseSort { get; set; } = false;
    
    public static PluginConfig FromJson(PluginConfigModel pluginConfigModel) => new(
        pluginConfigModel.Enabled,
        SaberValueTransforms.FromSerializedName(pluginConfigModel.CurrentlySelectedSaber),
        SaberValueTransforms.FromSerializedName(pluginConfigModel.CurrentlySelectedTrail),
        pluginConfigModel.DisableWhiteTrail,
        pluginConfigModel.OverrideTrailDuration,
        pluginConfigModel.TrailDuration,
        pluginConfigModel.OverrideTrailWidth,
        pluginConfigModel.TrailWidth,
        pluginConfigModel.OverrideSaberLength,
        pluginConfigModel.SaberLength,
        pluginConfigModel.OverrideSaberWidth,
        pluginConfigModel.SaberWidth,
        pluginConfigModel.EnableCustomEvents);

    private PluginConfig(
        bool enabled,
        SaberValue currentlySelectedSaber,
        SaberValue currentlySelectedTrail,
        bool disableWhiteTrail,
        bool overrideTrailDuration,
        float trailDuration,
        bool overrideTrailWidth,
        float trailWidth,
        bool overrideSaberLength,
        float saberLength,
        bool overrideSaberWidth,
        float saberWidth,
        bool enableCustomEvents)
    {
        Enabled = enabled;
        CurrentlySelectedSaber = currentlySelectedSaber;
        CurrentlySelectedTrail = currentlySelectedTrail;
        DisableWhiteTrail = disableWhiteTrail;
        OverrideTrailDuration = overrideTrailDuration;
        TrailDuration = trailDuration;
        OverrideTrailWidth = overrideTrailWidth;
        TrailWidth = trailWidth;
        OverrideSaberLength = overrideSaberLength;
        SaberLength = saberLength;
        OverrideSaberWidth = overrideSaberWidth;
        SaberWidth = saberWidth;
        EnableCustomEvents = enableCustomEvents;
    }
}