using System.Linq;
using CustomSabersLite.Components;
using CustomSabersLite.Models;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Services;

internal class TrailFactory
{
    private readonly GameResourcesProvider gameResourcesProvider;
    private readonly DiContainer container;
    
    public TrailFactory(GameResourcesProvider gameResourcesProvider, DiContainer container)
    {
        this.gameResourcesProvider = gameResourcesProvider;
        this.container = container;
    }

    private const int DefaultSamplingFrequency = 120;
    private const int DefaultGranularity = 45;

    /// <summary>
    /// Adds trails to a custom saber.
    /// </summary>
    /// <param name="saber">The saber to add the trails to.</param>
    /// <param name="trails">The trail data to create the trails with.</param>
    /// <param name="intensity">The alpha of the trail. Only works with certain shaders.</param>
    /// <returns>An array containing the new trail instances. Returns an empty array if none are created.</returns>
    public LiteSaberTrail[] AddTrailsTo(ILiteSaber saber, ITrailData[] trails, float intensity) => trails
        .Select(trail => AddCustomTrailTo(saber.GameObject, trail, intensity))
        .ToArray();

    private LiteSaberTrail AddCustomTrailTo(
        GameObject saberObject,
        ITrailData trailData,
        float intensity)
    {
        var trailTransformContainer = new GameObject("TrailTransformContainer") { layer = 12 }.transform;
        trailTransformContainer.SetParent(saberObject.transform, false);
        trailTransformContainer.localPosition = Vector3.zero;
        
        var trailTop = new GameObject("LiteSaberTrailTop") { layer = 12 }.transform;
        trailTop.SetParent(trailTransformContainer, false);
        trailTop.localPosition = trailData.TrailTopOffset;
        
        var trailBottom = new GameObject("LiteSaberTrailBottom") { layer = 12 }.transform;
        trailBottom.SetParent(trailTransformContainer, false);
        trailBottom.localPosition = trailData.TrailBottomOffset;

        var trailInitData = new LiteSaberTrail.InitData(trailTop, trailBottom, trailData);
        var trail = container.InstantiateComponent<LiteSaberTrail>(saberObject, [trailInitData]);
        var baseColor = (trailData.CustomColor * trailData.ColorMultiplier) with { a = intensity };

        trail._trailDuration = trailData.LengthSeconds;
        trail._samplingFrequency = DefaultSamplingFrequency;
        trail._granularity = DefaultGranularity;
        trail._color = baseColor;
        trail._trailRenderer = gameResourcesProvider.CreateNewSaberTrailRenderer();
        if (trailData.Material != null)
        {
            trail._trailRenderer._meshRenderer.material = trailData.Material;
            trail._trailRenderer._meshRenderer.material.color = baseColor;
        }

        return trail;
    }

    public DefaultTrailData CreateDefaultTrailData() => new(gameResourcesProvider.DefaultTrailMaterial);
}