using System;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;
using static UnityEngine.Object;

namespace CustomSabersLite.Models;

internal class SaberInstanceSet : IDisposable
{
    private readonly GameObject? root;

    public ILiteSaber? LeftSaber { get; }
    public ILiteSaber? RightSaber { get; }
    public ITrailData[] LeftTrails { get; }
    public ITrailData[] RightTrails { get; }
    
    /// <summary>
    /// Create a saber set from existing sabers and trails. The sabers should not have any parent objects.
    /// </summary>
    /// <param name="leftSaber">A single, existing saber</param>
    /// <param name="rightSaber">A single, existing saber</param>
    /// <param name="leftTrails">The trails to use on the left saber</param>
    /// <param name="rightTrails">The trails to use on the right saber</param>
    public SaberInstanceSet(
        ILiteSaber? leftSaber, ILiteSaber? rightSaber, ITrailData[] leftTrails, ITrailData[] rightTrails) =>
        (LeftSaber, RightSaber, LeftTrails, RightTrails) = (leftSaber, rightSaber, leftTrails, rightTrails);
    
    /// <summary>
    /// Instantiate a new saber instance from a saber prefab 
    /// </summary>
    /// <param name="saberPrefab">The root of the saber prefab</param>
    public SaberInstanceSet(GameObject saberPrefab)
    {
        root = Instantiate(saberPrefab);
        LeftSaber = new CustomLiteSaber(root.transform.Find("LeftSaber").gameObject);
        RightSaber = new CustomLiteSaber(root.transform.Find("RightSaber").gameObject);
        LeftTrails = [];
        RightTrails = [];
    }
    
    /// <summary>
    /// Creates a new saber set with the current sabers and the provided trails.
    /// This will not destroy any previous trail data.
    /// </summary>
    /// <param name="leftTrails">The trails to use on the left saber</param>
    /// <param name="rightTrails">The trails to use on the right saber</param>
    /// <returns></returns>
    public SaberInstanceSet WithTrails(ITrailData[] leftTrails, ITrailData[] rightTrails) => 
        new(root, LeftSaber, RightSaber, leftTrails, rightTrails);

    private SaberInstanceSet(
        GameObject? saberRoot,
        ILiteSaber? leftSaber,
        ILiteSaber? rightSaber,
        ITrailData[] leftTrails,
        ITrailData[] rightTrails) =>
        (root, LeftSaber, RightSaber, LeftTrails, RightTrails) =
        (saberRoot, leftSaber, rightSaber, leftTrails, rightTrails);
    
    public ILiteSaber? GetSaberForType(SaberType type) => type == SaberType.SaberA ? LeftSaber : RightSaber;
    public ITrailData[] GetTrailsForType(SaberType type) => type == SaberType.SaberA ? LeftTrails : RightTrails;
    
    public void Dispose()
    {
        LeftSaber?.Destroy();
        RightSaber?.Destroy();
        if (root != null) root.Destroy();
    }
}