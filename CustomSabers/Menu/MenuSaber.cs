using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities.Extensions;
using SabersCore.Components;
using SabersCore.Models;
using SabersCore.Services;
using SabersCore.Utilities.Common;
using UnityEngine;

namespace CustomSabersLite.Menu;

internal class MenuSaber
{
    private readonly PluginConfig config;
    private readonly ITrailFactory trailFactory;
    private readonly Transform parent;
    
    private MenuSaber(PluginConfig config, ITrailFactory trailFactory)
    {
        this.config = config;
        this.trailFactory = trailFactory;

        parent = new GameObject($"{nameof(CustomSabersLite)}{nameof(MenuSaber)}").transform;
        parent.gameObject.SetActive(false);
    }

    private ISaber? saberInstance;
    private CustomSaberTrail[] trailInstances = [];

    public Transform Parent => parent;
    
    public void ReplaceSaber(ISaber? newSaber, ITrailData[] newTrails)
    {
        if (newSaber is null) return;

        newSaber.SetParent(parent);
        newSaber.GameObject.GetComponentsInChildren<Collider>().ForEach(c => c.enabled = false);

        trailInstances = trailFactory.AddTrailsTo(newSaber, newTrails, 1f);
        saberInstance = newSaber;
    }

    public void UpdateTrails()
    {
        trailInstances.ConfigureTrails(new(
            config.DisableWhiteTrail,
            config.OverrideTrailWidth,
            config.TrailWidth,
            config.OverrideTrailDuration,
            config.TrailDuration));
    }

    public void UpdateSaberScale(float length, float width)
    {
        if (saberInstance is null) return;
        saberInstance.SetLength(length);
        saberInstance.SetWidth(width);
    }

    public void SetParent(Transform t)
    {
        parent.SetParent(t, false);
    }

    public void SetColor(Color color)
    {
        saberInstance?.SetColor(color);
        trailInstances.ForEach(t => t.SetColor(color));
    }

    public void SetActive(bool active)
    {
        parent.gameObject.SetActive(active);
    }
}
