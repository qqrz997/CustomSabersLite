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
            var trail = CreateTrail(saberObject, trailsData[i], intensity);
            trail.ConfigureTrail(config, i == 0);
            trails.Add(trail);
        }
        return [..trails];
    }

    private LiteSaberTrail CreateTrail(GameObject saberObject, CustomTrailData trailData, float intensity)
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
        var top = new GameObject().transform;
        var bottom = new GameObject().transform;

        top.SetParent(defaultSaber._saberBladeTopTransform.parent);
        bottom.SetParent(defaultSaber._saberBladeBottomTransform.parent);

        top.position = defaultSaber._saberBladeTopTransform.position;
        bottom.position = defaultSaber._saberBladeBottomTransform.position;

        var trailData = new CustomTrailData(
            top, bottom,
            TrailRendererPrefab._meshRenderer.material,
            defaultSaber.saberType == SaberType.SaberA ? CustomSaber.ColorType.LeftSaber : CustomSaber.ColorType.RightSaber,
            Color.white, Color.white,
            TrailUtils.DefaultDuration);

        var trail = CreateTrail(saberObject, trailData, intensity);
        trail.ConfigureTrail(config, true);

        return trail;
    }
}
