using CustomSabersLite.Components.Game;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities;
using CustomSabersLite.Utilities.Extensions;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.UI.Views.Saber_List;
internal class MenuSaber(Transform parent)
{
    [Inject] private readonly CSLConfig config;
    [Inject] private readonly InternalResourcesProvider resourcesProvider;

    private readonly Transform saberParent = parent;
    private LiteSaber saberInstance;
    private readonly List<LiteSaberTrail> trailInstances = [];

    private SaberTrailRenderer TrailRendererPrefab => resourcesProvider.SaberTrailRenderer;

    public void ReplaceSaber(LiteSaber newSaber)
    {
        trailInstances.Clear();
        saberInstance?.gameObject.DestroyImmediate();
        saberInstance = newSaber;
        
        if (!saberInstance) return;

        saberInstance.SetParent(saberParent);
        foreach (var collider in saberInstance.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false; // todo - is there a way to stop the colliders messing with the menu pointers
        }

        foreach (var trail in saberInstance.InstanceTrails)
        {
            trailInstances.Add(CreateTrail(saberInstance.gameObject, trail));
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

            trail.ConfigureTrail(config, primaryTrail);

            // todo - use custom default trail
            trail.enabled = config.TrailType == TrailType.Custom;

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

    private LiteSaberTrail CreateTrail(GameObject parent, CustomTrailData trailData)
    {
        var trail = parent.AddComponent<LiteSaberTrail>();

        trail._trailDuration = trailData.Length;
        trail._samplingFrequency = 120;
        trail._granularity = 45;
        trail._color = trailData.Color * trailData.ColorMultiplier;
        trail._trailRenderer = GameObject.Instantiate(TrailRendererPrefab, Vector3.zero, Quaternion.identity);
        trail._trailRenderer._meshRenderer.material = trailData.Material;
        trail._trailRenderer._meshRenderer.material.color = trailData.Color * trailData.ColorMultiplier;

        trail.Init(trailData);

        return trail;
    }
}
