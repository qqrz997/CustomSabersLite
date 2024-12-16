using CustomSabersLite.Components;
using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using CustomSabersLite.Utilities.Services;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.UI;

internal class MenuSaber
{
    private readonly CSLConfig config;
    private readonly TrailFactory trailFactory;
    private readonly SaberType saberType;

    private readonly GameObject gameObject;
    private readonly GameObject defaultTrailObject;
    private readonly LiteSaberTrail defaultTrail;

    private MenuSaber(CSLConfig config, TrailFactory trailFactory, Transform saberParent, SaberType saberType)
    {
        this.config = config;
        this.trailFactory = trailFactory;
        this.saberType = saberType;

        gameObject = new("MenuLiteSaber");
        gameObject.SetActive(false);
        gameObject.transform.SetParent(saberParent, false);
        defaultTrailObject = new("DefaultTrail");
        defaultTrailObject.transform.SetParent(gameObject.transform, false);
        defaultTrail = trailFactory.CreateDefaultTrail(defaultTrailObject, saberType, 1f);
    }

    private ILiteSaber? liteSaberInstance;
    private LiteSaberTrail[] trailInstances = [];

    public void ReplaceSaber(ILiteSaber? newSaber)
    {
        liteSaberInstance?.Destroy();
        trailInstances.ForEach(t => { if (t && t._trailRenderer) t._trailRenderer.gameObject.Destroy(); });
        if (newSaber == null) return;

        newSaber.SetParent(gameObject.transform);
        newSaber.GameObject.GetComponentsInChildren<Collider>().ForEach(c => c.enabled = false);

        trailInstances = trailFactory.CreateTrail(newSaber, saberType, TrailType.Custom);
        liteSaberInstance = newSaber;
    }

    public void UpdateTrails()
    {
        defaultTrail.ConfigureTrail(config, true);
        defaultTrail.enabled = !config.OverrideTrailDuration ? config.TrailType == TrailType.Vanilla
            : config.TrailDuration > 0 && config.TrailType == TrailType.Vanilla;

        for (int i = 0; i < trailInstances.Length; i++)
        {
            if (trailInstances[i])
            {
                trailInstances[i].ConfigureTrail(config, i == 0);
                trailInstances[i].enabled = !config.OverrideTrailDuration ? config.TrailType == TrailType.Custom
                    : config.TrailDuration > 0 && config.TrailType == TrailType.Custom;
            }
        }
    }

    public void UpdateSaberScale(float length, float width)
    {
        if (liteSaberInstance == null) return;
        liteSaberInstance.SetLength(length);
        liteSaberInstance.SetWidth(width);
        defaultTrailObject.transform.localScale = defaultTrailObject.transform.localScale with { z = length };
    }

    public void SetColor(Color color)
    {
        defaultTrail.SetColor(color);
        liteSaberInstance?.SetColor(color);
        trailInstances.ForEach(t => t.SetColor(color));
    }

    public void SetActive(bool active) =>
        gameObject.SetActive(liteSaberInstance != null && active);

    public class Factory : PlaceholderFactory<Transform, SaberType, MenuSaber> { }
}
