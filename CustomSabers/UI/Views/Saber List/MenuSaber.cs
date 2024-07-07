using CustomSabersLite.Components.Game;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities;
using CustomSabersLite.Utilities.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.UI.Views.Saber_List;
internal class MenuSaber
{
    private readonly CSLConfig config;
    private readonly Transform saberParent;
    private readonly SaberTrailRenderer trailRendererPrefab;

    public MenuSaber(CSLConfig config, Transform parent, SaberTrailRenderer trailRendererPrefab)
    {
        this.config = config;
        saberParent = parent;
        this.trailRendererPrefab = trailRendererPrefab;
    }

    private LiteSaber saberInstance;
    private List<LiteSaberTrail> trailInstances;

    public void ReplaceSaber(LiteSaber newSaber)
    {
        saberInstance?.gameObject.DestroyImmediate();
        saberInstance = newSaber;
        if (!saberInstance) return;

        saberInstance.SetParent(saberParent);
        saberInstance.gameObject.name = "Menu Saber Left";
        var trailData = saberInstance.InstanceTrails;
        trailInstances.Clear();
        for (var i = 0; i < trailData.Length; i++)
            trailInstances.Add(CreateTrail(saberInstance.gameObject, trailData[i], primaryTrail: i == 0));
    }

    private LiteSaberTrail CreateTrail(GameObject parent, CustomTrailData trailData, bool primaryTrail = false)
    {
        var trail = parent.AddComponent<LiteSaberTrail>();

        if (primaryTrail && config.OverrideTrailWidth)
            trailData.Bottom.position = trailData.GetOverrideWidthBottom(config.TrailWidth);

        trail._trailDuration = trailData.Length;
        trail._samplingFrequency = 120;
        trail._granularity = 45;
        trail._color = trailData.Color;
        trail._trailRenderer = GameObject.Instantiate(trailRendererPrefab, Vector3.zero, Quaternion.identity);
        trail._trailRenderer._meshRenderer.material = trailData.Material;
        trail._trailRenderer._meshRenderer.material.color = trailData.Color;

        trail.Setup(trailData.Top, trailData.Bottom);
        trail.ConfigureTrail(config);

        return trail;
    }
}
