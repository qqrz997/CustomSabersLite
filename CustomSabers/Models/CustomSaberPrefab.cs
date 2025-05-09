using CustomSabersLite.Utilities.Extensions;
using UnityEngine;
using static CustomSabersLite.Utilities.Common.CustomTrailUtils;

namespace CustomSabersLite.Models;

internal class CustomSaberPrefab : ISaberPrefab
{
    private readonly GameObject prefab;
    private readonly ITrailData[] leftTrails = [];
    private readonly ITrailData[] rightTrails = [];

    public CustomSaberPrefab(GameObject prefab)
    {
        this.prefab = prefab;

        var leftSaber = prefab.transform.Find("LeftSaber");
        if (leftSaber != null) leftTrails = GetTrailsFromCustomSaber(leftSaber.gameObject);
        else Logger.Warn($"Prefab \"{prefab.name}\" is missing a LeftSaber GameObject");
        
        var rightSaber = prefab.transform.Find("RightSaber");
        if (rightSaber != null) rightTrails = GetTrailsFromCustomSaber(rightSaber.gameObject); 
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