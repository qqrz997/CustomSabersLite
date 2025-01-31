using System;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CustomSabersLite.Models;

internal class SaberInstanceSet : IDisposable
{
    private readonly GameObject? root;

    public ILiteSaber? LeftSaber { get; }
    public ILiteSaber? RightSaber { get; }
    
    /// <summary>
    /// Create a saber set from existing <see cref="ILiteSaber"/>s.
    /// </summary>
    public SaberInstanceSet(ILiteSaber leftSaber, ILiteSaber rightSaber) =>
        (LeftSaber, RightSaber) = (leftSaber, rightSaber);

    /// <summary>
    /// Create a saber set from a custom saber prefab. Must be the root object of the prefab.
    /// </summary>
    public static SaberInstanceSet FromPrefab(GameObject prefab, CustomSaberType customSaberType)
    {
        // TODO: CustomSaberType is only used to get the custom trails, use polymorphism instead
        var saberInstanceSet = new SaberInstanceSet(prefab, customSaberType);
        
        if (saberInstanceSet.LeftSaber == null) Logger.Warn("Provided prefab doesn't have a left saber");
        if (saberInstanceSet.RightSaber == null) Logger.Warn("Provided prefab doesn't have a right saber");
        
        return saberInstanceSet;
    }
    
    public ILiteSaber? GetSaberForType(SaberType type) => type == SaberType.SaberA ? LeftSaber : RightSaber;

    public void Dispose()
    {
        LeftSaber?.Destroy();
        RightSaber?.Destroy();
        if (root != null) root.Destroy();
    }
    
    private SaberInstanceSet(GameObject prefab, CustomSaberType saberType)
    {
        root = Object.Instantiate(prefab);
        var left = root.transform.Find("LeftSaber")?.gameObject;
        var right = root.transform.Find("RightSaber")?.gameObject;
        if (left != null) LeftSaber = new CustomLiteSaber(left, saberType);
        if (right != null) RightSaber = new CustomLiteSaber(right, saberType);
    }
}