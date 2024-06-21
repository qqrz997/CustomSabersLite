using System.Collections.Generic;
using UnityEngine;
using CustomSabersLite.Data;
using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities;

namespace CustomSabersLite.Components.Game;
internal class TrailManager(CSLConfig config, GameplayCoreSceneSetupData gameplaySetupData)
{
    private readonly CSLConfig config = config;
    private readonly float saberTrailIntensity = gameplaySetupData.playerSpecificSettings.saberTrailIntensity;

    private SaberTrailRenderer defaultTrailRendererPrefab;
    private int defaultSamplingFrequency;
    private int defaultGranularity;
    private TrailElementCollection defaultTrailElementCollection;

    /// <summary>
    /// Sets up custom trails for a custom saber
    /// </summary>
    /// <returns>if no suitable trail is created, a custom trail using the default trail material is created instead</returns>
    public LiteSaberTrail[] CreateTrail(Saber defaultSaber, SaberTrail defaultTrail, Color saberTrailColor, LiteSaber customSaber, CustomTrailData[] customTrailData)
    {
        if (config.TrailType == TrailType.None)
        {
            return [];
        }

        var saberObject = customSaber.gameObject;
        defaultTrailRendererPrefab = defaultTrail._trailRendererPrefab;
        defaultSamplingFrequency = defaultTrail._samplingFrequency;
        defaultGranularity = defaultTrail._granularity;
        defaultTrailElementCollection = defaultTrail._trailElementCollection;

        if (config.TrailType == TrailType.Vanilla || customTrailData is null)
        {
            return [SetupTrailUsingDefaultMaterial(saberObject, defaultSaber, saberTrailColor)];
        }

        var trails = new List<LiteSaberTrail>();

        for (var i = 0; i < customTrailData.Length; i++)
        {
            trails.Add(i == 0 ? SetupTrail(saberObject, customTrailData[i])
                : SetupSecondaryTrail(saberObject, customTrailData[i]));
        }

        return [.. trails];
    }

    private LiteSaberTrail SetupTrailUsingDefaultMaterial(GameObject saberObject, Saber defaultSaber, Color saberTrailColor) =>
        SetupTrail(saberObject, TrailFromDefaultSaber(defaultSaber, saberTrailColor));

    public CustomTrailData TrailFromDefaultSaber(Saber defaultSaber, Color saberTrailColor)
    {
        // Make new transforms based on the default ones, because we cannot modify the default transforms
        var trailTop = GameObject.Instantiate(new GameObject()).transform;
        var trailBottom = GameObject.Instantiate(new GameObject()).transform;
        trailTop.SetPositionAndRotation(defaultSaber._saberBladeTopTransform.position, Quaternion.identity);
        trailBottom.SetPositionAndRotation(defaultSaber._saberBladeBottomTransform.position, Quaternion.identity);
        return new CustomTrailData(trailTop, trailBottom, new Material(defaultTrailRendererPrefab._meshRenderer.material), CustomSaber.ColorType.CustomColor, saberTrailColor, Color.white, TrailUtils.DefaultDuration);
    }

    private LiteSaberTrail SetupTrail(GameObject saberObject, CustomTrailData customTrailData)
    {
        var trail = saberObject.gameObject.AddComponent<LiteSaberTrail>();

        if (config.OverrideTrailWidth)
        {
            var trailTop = customTrailData.Top.position;
            var trailBottom = customTrailData.Bottom.position;
            var distance = Vector3.Distance(trailTop, trailBottom);
            var width = distance > 0 ? config.TrailWidth / 100f / distance : 1f;

            customTrailData.Bottom.position = Vector3.LerpUnclamped(trailTop, trailBottom, width);
        }
        return InitTrail(customTrailData, trail);
    }

    private LiteSaberTrail SetupSecondaryTrail(GameObject saberObject, CustomTrailData customTrailData)
    {
        var trail = saberObject.gameObject.AddComponent<LiteSaberTrail>();
        return InitTrail(customTrailData, trail);
    }

    private LiteSaberTrail InitTrail(CustomTrailData trailData, LiteSaberTrail trail)
    {
        trail._trailDuration = trailData.Length;
        trail._samplingFrequency = defaultSamplingFrequency;
        trail._granularity = defaultGranularity;
        trail._color = trailData.Color;
        trail._trailRenderer = GameObject.Instantiate(defaultTrailRendererPrefab, Vector3.zero, Quaternion.identity);
        trail._trailRenderer._meshRenderer.material = trailData.Material;
        trail._trailRenderer._meshRenderer.material.color = trailData.Color.ColorWithAlpha(saberTrailIntensity);
        trail._trailElementCollection = defaultTrailElementCollection;

        trail.Setup(trailData.Top, trailData.Bottom);
        trail.ConfigureTrail(config);

        return trail;
    }
}
