using CustomSabersLite.Components.Game;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities;
using CustomSabersLite.Utilities.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace CustomSabersLite.UI.Views.Saber_List;
internal class MenuSaber(CSLConfig config, Transform parent, SaberTrailRenderer trailRendererPrefab)
{
    private readonly CSLConfig config = config;
    private readonly Transform saberParent = parent;
    private readonly SaberTrailRenderer trailRendererPrefab = trailRendererPrefab;

    private LiteSaber saberInstance;
    private readonly List<LiteSaberTrail> trailInstances = [];
    private Color cachedColor = Color.cyan;

    public void ReplaceSaber(LiteSaber newSaber)
    {
        saberInstance?.gameObject.DestroyImmediate();
        saberInstance = newSaber;
        if (!saberInstance) return;

        saberInstance.SetParent(saberParent);
        saberInstance.gameObject.name = "Menu LiteSaber";
        UpdateTrails();
    }

    public void UpdateTrails()
    {
        if (!saberInstance) return;

        foreach (var trail in trailInstances)
        {
            Object.DestroyImmediate(trail);
        }
        trailInstances.Clear();

        var trailData = saberInstance.InstanceTrails;

        for (var i = 0; i < trailData.Length; i++)
        {
            var trail = CreateTrail(saberInstance.gameObject, trailData[i], primaryTrail: i == 0);
            trailInstances.Add(trail);
        }

        foreach (var trail in trailInstances)
        {
            trail.ConfigureTrail(config);
            var width = trail.GetTrailWidth(trail._movementData.lastAddedData);
            trail.SetColor(cachedColor);
        }
    }

    public void SetColor(Color color)
    {
        cachedColor = color;
        saberInstance?.SetColor(color);
        foreach (var trail in trailInstances) trail.SetColor(color);
    }

    public void SetActive(bool active) => saberInstance?.gameObject.SetActive(active);

    private LiteSaberTrail CreateTrail(GameObject parent, CustomTrailData trailData, bool primaryTrail = false)
    {
        var trail = parent.AddComponent<LiteSaberTrail>();

        if (primaryTrail && config.OverrideTrailWidth)
            trailData.Bottom.position = trailData.GetOverrideWidthBottom(config.TrailWidth);

        trail._trailDuration = trailData.Length;
        trail._samplingFrequency = 120;
        trail._granularity = 45;
        trail._color = trailData.Color * trailData.ColorMultiplier;
        trail._trailRenderer = GameObject.Instantiate(trailRendererPrefab, Vector3.zero, Quaternion.identity);
        trail._trailRenderer._meshRenderer.material = trailData.Material;
        trail._trailRenderer._meshRenderer.material.color = trailData.Color * trailData.ColorMultiplier;

        trail.Init(trailData);
        trail.ConfigureTrail(config);

        return trail;
    }
}
