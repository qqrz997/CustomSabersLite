using SiraUtil.Interfaces;
using UnityEngine;
using Zenject;
using CustomSabersLite.Utilities;
using CustomSabersLite.Utilities.Services;
using CustomSabersLite.Models;

namespace CustomSabersLite.Components;

#pragma warning disable IDE0031 // Use null propagation

internal class LiteSaberModelController : SaberModelController, IColorable, IPreSaberModelInit
{
    [Inject] private readonly LevelSaberManager levelSaberManager = null!;
    [Inject] private readonly TrailFactory trailFactory = null!;
    [Inject] private readonly SaberFactory saberFactory = null!;
    [Inject] private readonly SaberEventService saberEventService = null!;
    [Inject] private readonly ColorManager colorManager = null!;
    [Inject] private readonly GameplayCoreSceneSetupData gameplaySetupData = null!;

    private ILiteSaber? liteSaberInstance;
    private LiteSaberTrail[] customTrailInstances = [];

    private Color? color;
    public Color Color
    {
        get => color.GetValueOrDefault(); 
        set
        {
            color = value;
            liteSaberInstance?.SetColor(value);
            customTrailInstances?.ForEach(t => t.SetColor(value));
        }
    }

    public bool PreInit(Transform parent, Saber saber)
    {
        CustomSaberInit(parent, saber);
        return false;
    }

    private async void CustomSaberInit(Transform parent, Saber saber)
    {
        transform.SetParent(parent, false);
        transform.position = parent.position;
        transform.rotation = parent.rotation;

        var saberData = await levelSaberManager.LevelSaberInstance;
        liteSaberInstance = saberFactory.Create(saber.saberType, saberData);

        if (liteSaberInstance == null)
        {
            Logger.Error("Something went wrong when getting the custom saber instance");
            return;
        }

        float intensity = gameplaySetupData.playerSpecificSettings.saberTrailIntensity;
        customTrailInstances = trailFactory.CreateTrail(liteSaberInstance, saber.saberType, intensity);

        liteSaberInstance.SetParent(transform);
        
        if (liteSaberInstance.EventManager != null)
        {
            saberEventService.InitializeEventManager(liteSaberInstance.EventManager, saber.saberType);
        }

        Color = colorManager.ColorForSaberType(saber.saberType);
    }
}
