using System.Linq;
using CustomSabersLite.Components;
using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using CustomSabersLite.Utilities.Services;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Menu;

internal class MenuSaber
{
    private readonly CslConfig config;
    private readonly TrailFactory trailFactory;
    private readonly Transform parent;
    
    private MenuSaber(CslConfig config, TrailFactory trailFactory)
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
        liteSaberInstance?.Destroy();
        trailInstances.ForEach(t => { if (t && t._trailRenderer) t._trailRenderer.gameObject.Destroy(); });
        
        if (newSaber is null) return;

        newSaber.SetParent(parent);
        newSaber.GameObject.GetComponentsInChildren<Collider>().ForEach(c => c.enabled = false);

        trailInstances = trailFactory.AddTrailsTo(newSaber, newTrails, 1f);
        liteSaberInstance = newSaber;
    }

    public void UpdateTrails() => trailInstances.ConfigureTrails(config);

    public void UpdateSaberScale(float length, float width)
    {
        if (liteSaberInstance == null) return;
        liteSaberInstance.SetLength(length);
        liteSaberInstance.SetWidth(width);
    }

    public void SetParent(Transform t) => parent.SetParent(t, false); 
    
    public void SetColor(Color color)
    {
        liteSaberInstance?.SetColor(color);
        trailInstances.ForEach(t => t.SetColor(color));
    }

    public void SetActive(bool active) => parent.gameObject.SetActive(liteSaberInstance != null && active);
}
