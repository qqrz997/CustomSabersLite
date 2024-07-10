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
    private List<LiteSaberTrail> trailInstances = [];

    public void ReplaceSaber(LiteSaber newSaber)
    {
        trailInstances.Clear();
        saberInstance?.gameObject.DestroyImmediate();
        saberInstance = newSaber;
        
        if (!saberInstance) return;

        saberInstance.SetParent(saberParent);

        var trails = saberInstance.InstanceTrails;
        for (var i = 0; i < trails.Length; i++)
        {
            trailInstances.Add(CreateTrail(saberInstance.gameObject, trails[i], i == 0));
        }
    }

    public void UpdateTrails()
    {
        var primaryTrail = true;
        foreach (var trail in trailInstances)
        {
            if (!trail._trailRenderer)
            {
                continue;
            }
            var whiteSectionDuration = config.DisableWhiteTrail ? 0f : 0.03f;
            var (material, duration) = config.TrailType switch
            {
                TrailType.Custom => (trail.InstanceTrailData.Material, config.OverrideTrailDuration ? config.TrailDuration / 250f : trail.InstanceTrailData.Length),
                TrailType.Vanilla => (trailRendererPrefab._meshRenderer.material, config.OverrideTrailDuration ? config.TrailDuration / 250f : 0.4f),
                _ => (null, 0.0f)
            };

            trail.ConfigureTrail(config, primaryTrail);
            primaryTrail = false;
        }
    }

    public void SetColor(Color color)
    {
        saberInstance?.SetColor(color);
        foreach (var trail in trailInstances)
        {
            trail.SetColor(color);
        }
    }

    public void SetActive(bool active) => saberInstance?.gameObject.SetActive(active);

    private LiteSaberTrail CreateTrail(GameObject parent, CustomTrailData trailData, bool primaryTrail = false)
    {
        var trail = parent.AddComponent<LiteSaberTrail>();

        trail._trailDuration = trailData.Length;
        trail._samplingFrequency = 120;
        trail._granularity = 45;
        trail._color = trailData.Color * trailData.ColorMultiplier;
        trail._trailRenderer = GameObject.Instantiate(trailRendererPrefab, Vector3.zero, Quaternion.identity);
        trail._trailRenderer._meshRenderer.material = trailData.Material;
        trail._trailRenderer._meshRenderer.material.color = trailData.Color * trailData.ColorMultiplier;

        trail.Init(trailData);

        return trail;
    }
}
