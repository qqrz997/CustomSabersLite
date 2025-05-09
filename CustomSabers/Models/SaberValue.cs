namespace CustomSabersLite.Models;

internal abstract record SaberValue;

internal sealed record SaberHash(string Hash) : SaberValue;
internal sealed record DefaultSaberValue : SaberValue;
internal sealed record CustomTrailValue : SaberValue;
internal sealed record NoTrailValue : SaberValue;