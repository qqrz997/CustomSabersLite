using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace CustomSabersLite.Configuration;

internal class PluginConfigModel
{
    public bool Enabled { get; set; } = true;

    public string? CurrentlySelectedSaber { get; set; } = null;
    public string? CurrentlySelectedTrail { get; set; } = null;
    
    public bool DisableWhiteTrail { get; set; } = true;

    public bool OverrideTrailDuration { get; set; } = false;
    public float TrailDuration { get; set; } = 1f;

    public bool OverrideTrailWidth { get; set; } = false;
    public float TrailWidth { get; set; } = 1f;

    public bool OverrideSaberLength { get; set; } = false;
    public float SaberLength { get; set; } = 1f;
    
    public bool OverrideSaberWidth { get; set; } = false;
    public float SaberWidth { get; set; } = 1f;
        
    public bool EnableCustomEvents { get; set; } = true;

    public virtual void Changed() { }
}