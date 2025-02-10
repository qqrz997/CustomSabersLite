using CustomSabersLite.Utilities.Extensions;
using UnityEngine;
using static CustomSabersLite.Utilities.Common.CustomTrailUtils;

namespace CustomSabersLite.Models;

internal class WhackerPrefab : ISaberPrefab
{
    private readonly GameObject prefab;
    private readonly ITrailData[] leftTrails = [];
    private readonly ITrailData[] rightTrails = [];

    public WhackerPrefab(GameObject prefab)
    {
        this.prefab = prefab;

        var leftSaber = prefab.transform.Find("LeftSaber");
        if (leftSaber != null) leftTrails = GetTrailsFromWhacker(leftSaber.gameObject);
        else Logger.Warn($"Prefab \"{prefab.name}\" is missing a LeftSaber GameObject");
        
        var rightSaber = prefab.transform.Find("RightSaber");
        if (rightSaber != null) rightTrails = GetTrailsFromWhacker(rightSaber.gameObject); 
        else Logger.Warn($"Prefab \"{prefab.name}\" is missing a RightSaber GameObject");
    }

    public SaberInstanceSet Instantiate() => new(prefab);
    public ITrailData[] GetTrailsForType(SaberType saberType) => 
        saberType == SaberType.SaberA ? leftTrails : rightTrails;

    public void Dispose()
    {
        if (prefab != null) prefab.Destroy();
    }
}