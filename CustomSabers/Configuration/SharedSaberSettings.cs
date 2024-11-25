using CustomSabersLite.Models;

namespace CustomSabersLite.Configuration;

internal interface ISharedSaberSettings
{
    public bool DisableWhiteTrail { get; set; }
    public bool OverrideTrailDuration { get; set; }
    public int TrailDuration { get; set; }
    public bool OverrideTrailWidth { get; set; }
    public int TrailWidth { get; set; }
    public bool OverrideSaberLength { get; set; }
    public int SaberLength { get; set; }
    public bool OverrideSaberWidth { get; set; }
    public int SaberWidth { get; set; }
    public string TrailType { get; set; }
    public bool EnableCustomEvents { get; set; }

    internal static string[] PropertyNames { get; } =
    [
        nameof(DisableWhiteTrail),
        nameof(OverrideTrailDuration),
        nameof(TrailDuration),
        nameof(OverrideTrailWidth),
        nameof(TrailWidth),
        nameof(OverrideSaberLength),
        nameof(SaberLength),
        nameof(OverrideSaberWidth),
        nameof(SaberWidth),
        nameof(TrailType),
        nameof(EnableCustomEvents)
    ];
}
