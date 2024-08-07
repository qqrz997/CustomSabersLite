using SiraUtil.Interfaces;
using UnityEngine;
using Zenject;
using CustomSabersLite.Configuration;
using CustomSabersLite.Components.Managers;

namespace CustomSabersLite.Components.Game;

internal class LiteSaberModelController : SaberModelController, IColorable, IPreSaberModelInit
{
    [Inject] private readonly LevelSaberManager levelSaberManager;
    [Inject] private readonly TrailFactory trailFactory;
    [Inject] private readonly SaberFactory saberFactory;
    [Inject] private readonly EventManagerManager eventManagerManager;
    [Inject] private readonly ColorManager colorManager;
    [Inject] private readonly GameplayCoreSceneSetupData gameplaySetupData;

    private LiteSaber customSaberInstance;
    private LiteSaberTrail[] customTrailInstances = [];

    public Color Color { get => color.GetValueOrDefault(); set => SetColor(value); }
    private Color? color;

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

        if (!customSaberInstance)
        {
            Logger.Error("Something went wrong when getting the custom saber instance");
            return;
        }

        var intensity = gameplaySetupData.playerSpecificSettings.saberTrailIntensity;
        customTrailInstances = trailFactory.CreateTrail(customSaberInstance, saber.saberType, intensity);

        customSaberInstance.SetParent(transform);
        eventManagerManager.InitializeEventManager(customSaberInstance.EventManager, saber.saberType);

        SetColor(colorManager.ColorForSaberType(saber.saberType));
    }

    private void SetColor(Color color)
    {
        this.color = color;
        customSaberInstance?.SetColor(color);
        if (customTrailInstances != null)
        {
            foreach (var customTrail in customTrailInstances)
            {
                customTrail?.SetColor(color);
            }
        }
    }
}
