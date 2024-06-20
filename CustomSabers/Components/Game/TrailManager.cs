using System.Collections.Generic;
using System.Linq;
using CustomSaber;
using UnityEngine;
using CustomSabersLite.Data;
using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities;
using UnityEngine.UI;
using Newtonsoft.Json;

namespace CustomSabersLite.Components.Game;

/// <summary>
/// Manages an instance of a <seealso cref="LiteSaberTrail"/>
/// </summary>
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
    /// <returns>null if no trail is created</returns>
    public LiteSaberTrail[] CreateTrail(Saber defaultSaber, SaberTrail defaultTrail, Color saberTrailColor, LiteSaber customSaber)
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

        if (config.TrailType == TrailType.Vanilla)
        {
            return [SetupTrailUsingDefaultMaterial(saberObject, defaultSaber, saberTrailColor)];
        }
        var customTrailData = CustomTrailUtils.GetTrailFromCustomSaber(saberTrailColor, customSaber.Type, saberObject);

        if (customTrailData is null)
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
        return new CustomTrailData(trailTop, trailBottom, new Material(defaultTrailRendererPrefab._meshRenderer.material), saberTrailColor);
    }

    private LiteSaberTrail SetupTrail(GameObject saberObject, CustomTrailData customTrailData)
    {
        var trail = saberObject.gameObject.AddComponent<LiteSaberTrail>();

        if (config.OverrideTrailWidth)
        {
            var trailTop = customTrailData.TrailTop.position;
            var trailBottom = customTrailData.TrailBottom.position;
            var distance = Vector3.Distance(trailTop, trailBottom);
            var width = distance > 0 ? config.TrailWidth / 100f / distance : 1f;

            customTrailData.TrailBottom.position = Vector3.LerpUnclamped(trailTop, trailBottom, width);
        }
        return InitTrail(customTrailData, trail);
    }

    private LiteSaberTrail SetupSecondaryTrail(GameObject saberObject, CustomTrailData customTrailData)
    {
        var trail = saberObject.gameObject.AddComponent<LiteSaberTrail>();
        return InitTrail(customTrailData, trail);
    }

    private LiteSaberTrail InitTrail(CustomTrailData customTrailData, LiteSaberTrail trail)
    {
        trail._trailDuration = TrailUtils.ConvertedDuration(customTrailData.Length);
        trail._samplingFrequency = defaultSamplingFrequency;
        trail._granularity = defaultGranularity;
        trail._color = customTrailData.Color;
        trail._trailRenderer = GameObject.Instantiate(defaultTrailRendererPrefab, Vector3.zero, Quaternion.identity);
        trail._trailRenderer._meshRenderer.material = customTrailData.Material;
        trail._trailRenderer._meshRenderer.material.color = customTrailData.Color.ColorWithAlpha(saberTrailIntensity);
        trail._trailElementCollection = defaultTrailElementCollection;

        trail.Setup(customTrailData.TrailTop, customTrailData.TrailBottom);
        trail.ConfigureTrail(config);

        return trail;
    }
}
