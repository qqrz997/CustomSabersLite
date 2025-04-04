using CustomSabersLite.Components;
using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using CustomSabersLite.Services;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;

namespace CustomSabersLite.Menu;

internal class MenuSaber
{
    private readonly PluginConfig config;
    private readonly TrailFactory trailFactory;
    private readonly Transform parent;
    
    private MenuSaber(PluginConfig config, TrailFactory trailFactory)
    {
        this.config = config;
        this.trailFactory = trailFactory;

        parent = new GameObject("MenuLiteSaber").transform;
        parent.gameObject.SetActive(false);
    }

    private ILiteSaber? liteSaberInstance;
    private LiteSaberTrail[] trailInstances = [];

    public void ReplaceSaber(ILiteSaber? newSaber, ITrailData[] newTrails)
    {
        if (newSaber is null) return;

        newSaber.SetParent(parent);
        newSaber.GameObject.GetComponentsInChildren<Collider>().ForEach(c => c.enabled = false);

        trailInstances = trailFactory.AddTrailsTo(newSaber, newTrails, 1f);
        liteSaberInstance = newSaber;
    }

    public void UpdateTrails()
    {
        trailInstances.ConfigureTrails(config);
    }

    public void UpdateSaberScale(float length, float width)
    {
        if (liteSaberInstance is null) return;
        liteSaberInstance.SetLength(length);
        liteSaberInstance.SetWidth(width);
    }

    public void SetParent(Transform t)
    {
        parent.SetParent(t, false);
    }

    public void SetColor(Color color)
    {
        liteSaberInstance?.SetColor(color);
        trailInstances.ForEach(t => t.SetColor(color));
    }

    public void SetActive(bool active)
    {
        parent.gameObject.SetActive(active);
        liteSaberInstance?.GameObject.SetActive(active);
    }
}
