namespace CustomSabersLite.Configuration;

internal interface ISharedSaberSettings
{
    public bool DisableWhiteTrail { get; set; }
    public bool OverrideTrailDuration { get; set; }
    public float TrailDuration { get; set; }
    public bool OverrideTrailWidth { get; set; }
    public float TrailWidth { get; set; }
    public bool OverrideSaberLength { get; set; }
    public float SaberLength { get; set; }
    public bool OverrideSaberWidth { get; set; }
    public float SaberWidth { get; set; }
    // public string TrailType { get; set; }
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
        // nameof(TrailType),
        nameof(EnableCustomEvents)
    ];
}
