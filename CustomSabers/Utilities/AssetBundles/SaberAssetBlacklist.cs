using System.Linq;

namespace CustomSabersLite.Utilities.AssetBundles;

/// <summary>
/// This class contains a blacklist of saber files that, upon load, crash the game.
/// These sabers work on Beat Saber using the old version of unity used before v1.29.4
/// </summary>
internal class SaberAssetBlacklist
{
    private static readonly string[] saberNames = [
        "iSF-Curse",
        "iSF-NewYears2022",
        "iSF-NewYears2022-BlackHandle",
        "iSF-NSG-Astral Wand",
        "iSF-NSG-Chivalry",
        "iSF-NSG-Twizzler",
        "iSF-NSG-Zephyr",
        "iSF-Royals",
        "iSFxJoetastic-r99 from Apex Legends",
        "iSFxJoetastic-ThomasTheWankEngines",
        "iSFxJoetastic-ThomasTheWankEngines",
        "iSFxP1-Dyson(Particles)",
        "iSFxP1-DysonCC(NoParticles)",
        "iSFxP1-DysonCC(Particles)",
        "iSFxP1-DysonNoParticles",
        "iSFxTq-Blyze",
        "iSFxWk-Impurity"
    ];

    public static bool IsOnBlacklist(string saberName) =>
        saberNames.Any(saberName.Contains);
}
