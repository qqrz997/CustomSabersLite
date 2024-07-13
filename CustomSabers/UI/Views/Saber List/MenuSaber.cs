using CustomSabersLite.Components.Game;
using CustomSabersLite.Components.Managers;
using CustomSabersLite.Configuration;
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

    private LiteSaber saberInstance;
    private LiteSaberTrail[] trailInstances = [];

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
        for (var i = 0; i < trailInstances.Length; i++)
        {
            if (trailInstances[i]._trailRenderer)
            {
                trailInstances[i].ConfigureTrail(config, i == 0);
            }
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

    public class Factory : PlaceholderFactory<Transform, SaberType, MenuSaber> { }
}
