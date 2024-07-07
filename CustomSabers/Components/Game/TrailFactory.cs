using System.Collections.Generic;
using UnityEngine;
using CustomSabersLite.Data;
using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities;
using Zenject;
using CustomSabersLite.Utilities.Extensions;

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
            TrailType.Custom when trailsData is not null => CreateTrails(saberObject, trailsData, intensity),
            TrailType.Custom when trailsData is null => [CreateDefaultTrail(defaultSaber, saberObject, intensity)],
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

        if (primaryTrail && config.OverrideTrailWidth)
            trailData.Bottom.position = trailData.GetOverrideWidthBottom(config.TrailWidth);

        trail._trailDuration = trailData.Length;
        trail._samplingFrequency = defaultSamplingFrequency;
        trail._granularity = defaultGranularity;
        trail._color = trailData.Color;
        trail._trailRenderer = Object.Instantiate(TrailRendererPrefab, Vector3.zero, Quaternion.identity);
        trail._trailRenderer._meshRenderer.material = trailData.Material;
        trail._trailRenderer._meshRenderer.material.color = trailData.Color.ColorWithAlpha(intensity);
        trail._trailElementCollection = defaultTrailElementCollection;

        trail.Setup(trailData.Top, trailData.Bottom);
        trail.ConfigureTrail(config);

        return trail;
    }

    private LiteSaberTrail CreateDefaultTrail(Saber defaultSaber, GameObject saberObject, float intensity)
    {
        // Make new transforms based on the default ones, because we cannot modify the default transforms
        var trailData = new CustomTrailData(
            new GameObject().transform, new GameObject().transform,
            TrailRendererPrefab._meshRenderer.material,
            CustomSaber.ColorType.CustomColor, Color.white, Color.white,
            TrailUtils.DefaultDuration);
        trailData.Top.SetParent(defaultSaber._saberBladeTopTransform.parent);
        trailData.Bottom.SetParent(defaultSaber._saberBladeBottomTransform.parent);
        trailData.Top.position = defaultSaber._saberBladeTopTransform.position;
        trailData.Bottom.position = defaultSaber._saberBladeBottomTransform.position;
        return CreateTrail(saberObject, trailData, intensity, true);
    }
}
