using System.Diagnostics.CodeAnalysis;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;

namespace CustomSabersLite.Utilities.Extensions;

internal static class SaberValueExtensions
{
    public static bool TryGetSaberHash(this SaberValue saberValue, [NotNullWhen(true)] out SaberHash? saberHash)
    {
        saberHash = null;
        if (saberValue is SaberHash value)
        {
            saberHash = value;
        }
        return saberHash is not null;
    }
}

internal static class SaberValueTransforms
{
    public static string? GetSerializedName(this SaberValue saberValue) => saberValue switch
    {
        SaberHash saberHash => saberHash.Hash,
        DefaultSaberValue => "defaultSaber",
        CustomTrailValue => "customTrail",
        NoTrailValue => "noTrail",
        _ => null
    };

    public static SaberValue FromSerializedName(string? serializedName) => serializedName switch
    {
        "customTrail" => new CustomTrailValue(),
        "noTrail" => new NoTrailValue(),
        // could introduce a better string validation for these
        { Length: SaberHashing.HashLength } => new SaberHash(serializedName),
        _ => new DefaultSaberValue()
    };
}