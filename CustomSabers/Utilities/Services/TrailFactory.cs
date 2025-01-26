﻿using System.Collections.Generic;
using CustomSabersLite.Components;
using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;
using UnityEngine;

namespace CustomSabersLite.Utilities.Services;

internal class TrailFactory(CslConfig config, GameResourcesProvider gameResourcesProvider)
{
    private readonly CslConfig config = config;
    private readonly GameResourcesProvider gameResourcesProvider = gameResourcesProvider;

    private const int DefaultSamplingFrequency = 120;
    private const int DefaultGranularity = 45;

    /// <summary>
    /// Sets up custom trails for a custom saber
    /// </summary>
    /// <returns>if no suitable trail is created, a custom trail using the default trail material is created instead</returns>
    public LiteSaberTrail[] CreateTrail(ILiteSaber customSaber, SaberType saberType, float intensity = 1f) =>
        CreateTrail(customSaber, saberType, config.TrailType, intensity);

    public LiteSaberTrail[] CreateTrail(ILiteSaber customSaber, SaberType saberType, TrailType trailType, float intensity = 1f) => trailType switch
    {
        TrailType.Vanilla => [CreateDefaultTrail(customSaber.GameObject, saberType, intensity)],
        TrailType.Custom => CreateTrails(customSaber.GameObject, customSaber.TrailData, saberType, intensity),
        _ => []
    };

    private LiteSaberTrail[] CreateTrails(GameObject saberObject, CustomTrailData[] trailsData, SaberType saberType, float intensity)
    {
        var trails = new List<LiteSaberTrail>();
        for (int i = 0; i < trailsData.Length; i++)
        {
            var trail = CreateTrail(saberObject, trailsData[i], intensity);
            trail.ConfigureTrail(config, i == 0);
            trails.Add(trail);
        }
        return trails.Count > 0 ? trails.ToArray()
            : [CreateDefaultTrail(saberObject, saberType, intensity)];
    }

    private LiteSaberTrail CreateTrail(GameObject saberObject, CustomTrailData trailData, float intensity)
    {
        var trail = saberObject.AddComponent<LiteSaberTrail>();
        var baseColor = trailData.GetTrailColor() with { a = intensity };

        trail._trailDuration = trailData.Length;
        trail._samplingFrequency = DefaultSamplingFrequency;
        trail._granularity = DefaultGranularity;
        trail._trailRenderer = gameResourcesProvider.CreateNewSaberTrailRenderer();
        if (trail._trailRenderer != null)
        {
            trail._trailRenderer._meshRenderer.material = trailData.Material;
            if (trail._trailRenderer._meshRenderer.material != null)
            {
                trail._trailRenderer._meshRenderer.material.color = baseColor;
            }
        }
        trail._color = baseColor;
        trail.Init(trailData);

        return trail;
    }

    public LiteSaberTrail CreateDefaultTrail(GameObject saberObject, SaberType saberType, float intensity)
    {
        // Make new transforms based on the default ones, because we cannot modify the default transforms
        var top = new GameObject("Top").transform;
        var bottom = new GameObject("Bottom").transform;
        bottom.localPosition = Vector3.zero;
        top.localPosition = Vector3.forward;

        bottom.SetParent(saberObject.transform, false);
        top.SetParent(saberObject.transform, false);

        var trailData = new CustomTrailData(
            top, bottom,
            gameResourcesProvider.DefaultTrailMaterial,
            saberType == SaberType.SaberA ? CustomSaber.ColorType.LeftSaber : CustomSaber.ColorType.RightSaber,
            Color.white, Color.white,
            TrailUtils.DefaultDuration);

        var trail = CreateTrail(saberObject, trailData, intensity);
        trail.ConfigureTrail(config, true);

        return trail;
    }
}
