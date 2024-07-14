using CustomSabersLite.Components.Game;
using CustomSabersLite.Components.Managers;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.UI.Views.Saber_List;

internal class MenuSaber
{
    [Inject] private readonly CSLConfig config;
    [Inject] private readonly TrailFactory trailFactory;
    [Inject] private readonly Transform saberParent;
    [Inject] private readonly SaberType saberType;

    private GameObject gameObject;
    private LiteSaberTrail defaultTrail;

    [Inject] private void Construct()
    {
        gameObject = new GameObject("MenuLiteSaber");
        gameObject.SetActive(false);
        gameObject.transform.SetParent(saberParent, false);
        defaultTrail = trailFactory.CreateDefaultTrail(gameObject, saberType, 1f);
    }

    private LiteSaber saberInstance;
    private LiteSaberTrail[] trailInstances = [];

    public void ReplaceSaber(LiteSaber newSaber)
    {
        if (saberInstance)
        {
            saberInstance.gameObject.Destroy();
        }

        if (!newSaber)
        {
            return;
        }

        newSaber.SetParent(gameObject.transform);
        
        foreach (var collider in newSaber.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false; // todo - is there a way to stop the colliders messing with the menu pointers
        }

        trailInstances = trailFactory.CreateTrail(newSaber, saberType, TrailType.Custom);
        saberInstance = newSaber;
    }

    public void UpdateTrails()
    {
        defaultTrail.ConfigureTrail(config, true);
        defaultTrail.enabled = !config.OverrideTrailDuration ? config.TrailType == TrailType.Vanilla
            : config.TrailDuration > 0 && config.TrailType == TrailType.Vanilla;

        for (var i = 0; i < trailInstances.Length; i++)
        {
            if (trailInstances[i])
            {
                trailInstances[i].ConfigureTrail(config, i == 0);
                trailInstances[i].enabled = !config.OverrideTrailDuration ? config.TrailType == TrailType.Custom
                    : config.TrailDuration > 0 && config.TrailType == TrailType.Custom;
            }
        }
    }

    public void SetColor(Color color)
    {
        defaultTrail.SetColor(color);
        saberInstance?.SetColor(color);
        
        foreach (var trail in trailInstances)
        {
            trail.SetColor(color);
        }
    }

    public void SetActive(bool active) => 
        gameObject.SetActive(saberInstance && active);

    public class Factory : PlaceholderFactory<Transform, SaberType, MenuSaber> { }
}
