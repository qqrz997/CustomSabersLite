using System.Collections.Generic;
using UnityEngine;
using CustomSabersLite.Data;
using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities;
using Zenject;

namespace CustomSabersLite.Components.Game;

internal class TrailFactory
{
    [Inject] private readonly CSLConfig config;
    [Inject] private readonly InternalResourcesProvider resourcesProvider;

    private readonly Transform defaultTop = new GameObject().transform;
    private readonly Transform defaultBottom = new GameObject().transform;

    private SaberTrailRenderer TrailRendererPrefab => resourcesProvider.SaberTrailRenderer;

    private int defaultSamplingFrequency;
    private int defaultGranularity;
    private TrailElementCollection defaultTrailElementCollection;

    /// <summary>
    /// Sets up custom trails for a custom saber
    /// </summary>
    /// <returns>if no suitable trail is created, a custom trail using the default trail material is created instead</returns>
    public LiteSaberTrail[] CreateTrail(Saber defaultSaber, SaberTrail defaultTrail, LiteSaber customSaber, float intensity = 1f)
    {
        defaultSamplingFrequency = defaultTrail._samplingFrequency;
        defaultGranularity = defaultTrail._granularity;
        defaultTrailElementCollection = defaultTrail._trailElementCollection;

        var saberObject = customSaber.gameObject;
        var trailsData = customSaber.InstanceTrails;

        return config.TrailType switch
        {
            TrailType.Vanilla => [CreateDefaultTrail(defaultSaber, saberObject, intensity)],
            TrailType.Custom => CreateTrails(saberObject, trailsData, intensity),
            _ => []
        };
    }

    private LiteSaberTrail[] CreateTrails(GameObject saberObject, CustomTrailData[] trailsData, float intensity)
    {
        var trails = new List<LiteSaberTrail>();
        for (var i = 0; i < trailsData.Length; i++)
        {
            trails.Add(CreateTrail(saberObject, trailsData[i], intensity, primaryTrail: i == 0));
        }
        return [..trails];
    }

    private LiteSaberTrail CreateTrail(GameObject saberObject, CustomTrailData trailData, float intensity, bool primaryTrail = false)
    {
        var trail = saberObject.AddComponent<LiteSaberTrail>();

        trail._trailDuration = trailData.Length;
        trail._samplingFrequency = defaultSamplingFrequency;
        trail._granularity = defaultGranularity;
        trail._color = trailData.Color * trailData.ColorMultiplier;
        trail._trailRenderer = Object.Instantiate(TrailRendererPrefab, Vector3.zero, Quaternion.identity);
        trail._trailRenderer._meshRenderer.material = trailData.Material;
        trail._trailRenderer._meshRenderer.material.color = trailData.Color.ColorWithAlpha(intensity) * trailData.ColorMultiplier;
        trail._trailElementCollection = defaultTrailElementCollection;

        trail.Init(trailData);

        return trail;
    }

    private LiteSaberTrail CreateDefaultTrail(Saber defaultSaber, GameObject saberObject, float intensity)
    {
        // Make new transforms based on the default ones, because we cannot modify the default transforms
        defaultTop.SetParent(defaultSaber._saberBladeTopTransform.parent);
        defaultBottom.SetParent(defaultSaber._saberBladeBottomTransform.parent);

        defaultTop.position = defaultSaber._saberBladeTopTransform.position;
        defaultBottom.position = defaultSaber._saberBladeBottomTransform.position;

        var trailData = new CustomTrailData(
            defaultTop, defaultBottom,
            TrailRendererPrefab._meshRenderer.material,
            CustomSaber.ColorType.CustomColor, Color.white, Color.white,
            TrailUtils.DefaultDuration);

        return CreateTrail(saberObject, trailData, intensity, true);
    }
}
