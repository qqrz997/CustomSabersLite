using SiraUtil.Interfaces;
using UnityEngine;
using Zenject;
using CustomSabersLite.Utilities;
using CustomSabersLite.Utilities.Services;

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

    private LiteSaber? customSaberInstance;
    private LiteSaberTrail[] customTrailInstances = [];

    private Color? color;
    public Color Color
    {
        get => color.GetValueOrDefault(); 
        set
        {
            color = value;
            if (customSaberInstance != null) customSaberInstance.SetColor(value);
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

        var saberData = await levelSaberManager.SaberSetupTask;
        customSaberInstance = saberFactory.TryCreate(saber.saberType, saberData);

        if (customSaberInstance == null)
        {
            Logger.Error("Something went wrong when getting the custom saber instance");
            return;
        }

        var intensity = gameplaySetupData.playerSpecificSettings.saberTrailIntensity;
        customTrailInstances = trailFactory.CreateTrail(customSaberInstance, saber.saberType, intensity);

        customSaberInstance.SetParent(transform);
        saberEventService.InitializeEventManager(customSaberInstance.EventManager, saber.saberType);

        Color = colorManager.ColorForSaberType(saber.saberType);
    }
}
