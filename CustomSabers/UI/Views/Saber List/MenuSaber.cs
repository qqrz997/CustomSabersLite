using CustomSabersLite.Components.Game;
using CustomSabersLite.Components.Managers;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities;
using CustomSabersLite.Utilities.Extensions;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.UI.Views.Saber_List;

internal class MenuSaber(Transform parent, SaberType saberType)
{
    [Inject] private readonly CSLConfig config;
    [Inject] private readonly TrailFactory trailFactory;

    private readonly SaberType saberType = saberType;
    private readonly Transform saberParent = parent;

    private LiteSaber saberInstance;
    private LiteSaberTrail[] trailInstances = [];
    private TrailType lastTrailType = TrailType.Custom;

    public void ReplaceSaber(LiteSaber newSaber)
    {
        saberInstance?.gameObject.DestroyImmediate();
        saberInstance = newSaber;

        if (!saberInstance) return;

        saberInstance.SetParent(saberParent);
        foreach (var collider in saberInstance.GetComponentsInChildren<Collider>())
        {
            collider.enabled = false; // todo - is there a way to stop the colliders messing with the menu pointers
        }

        CreateNewTrails();
    }

    public void UpdateTrails()
    {
        if (!saberInstance)
        {
            return;
        }

        if (lastTrailType != config.TrailType)
        {
            CreateNewTrails();   
        }
        lastTrailType = config.TrailType;

        var primaryTrail = true;
        foreach (var trail in trailInstances)
        {
            if (!trail._trailRenderer)
            {
                continue;
            }

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

    private void CreateNewTrails()
    {
        foreach (var trail in trailInstances)
        {
            Object.Destroy(trail);
        }
        trailInstances = trailFactory.CreateTrail(saberInstance, saberType);
    }
}
