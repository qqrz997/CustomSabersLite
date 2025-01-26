using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using CustomSabersLite.Models;
using IPA.Config.Stores.Attributes;
using JetBrains.Annotations;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace CustomSabersLite.Configuration;

[UsedImplicitly]
internal class CslConfig
{
    public virtual bool Enabled { get; set; } = true;
    
    // TODO: use file hash as the key for saber data
    public virtual string? CurrentlySelectedSaber { get; set; } = null;

    public virtual bool DisableWhiteTrail { get; set; } = true;

    public virtual bool OverrideTrailDuration { get; set; } = false;
    public virtual float TrailDuration { get; set; } = 1f;

    public virtual bool OverrideTrailWidth { get; set; } = false;
    public virtual float TrailWidth { get; set; } = 1f;

    public virtual bool OverrideSaberLength { get; set; } = false;
    public virtual float SaberLength { get; set; } = 1f;
    
    public virtual bool OverrideSaberWidth { get; set; } = false;
    public virtual float SaberWidth { get; set; } = 1f;
        
    public virtual TrailType TrailType { get; set; } = TrailType.Custom;

    public virtual bool EnableCustomEvents { get; set; } = true;

    [Ignore] public bool EnableMenuSabers { get; set; } = false;
    [Ignore] public OrderBy OrderByFilter { get; set; } = OrderBy.Name;
    [Ignore] public string SearchFilter { get; set; } = string.Empty;
    [Ignore] public bool ReverseSort { get; set; } = false;

    // public virtual void OnReload() { }

    // public virtual void Changed() { }

    // public virtual void CopyFrom(CustomSaberConfig other) { }
}
