using CustomSabersLite.Models;

namespace CustomSabersLite.Configuration;

internal class SharedSaberSettings
{
    public bool DisableWhiteTrail;
    public bool OverrideTrailDuration;
    public int TrailDuration;
    public bool OverrideTrailWidth;
    public int TrailWidth;
    public TrailType TrailType;
    public bool EnableCustomEvents;

    internal static string[] PropertyNames =
    [
        nameof(DisableWhiteTrail),
        nameof(OverrideTrailDuration),
        nameof(TrailDuration),
        nameof(OverrideTrailWidth),
        nameof(TrailWidth),
        nameof(TrailType),
        nameof(EnableCustomEvents)
    ];
}
