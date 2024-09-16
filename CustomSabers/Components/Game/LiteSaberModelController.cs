using SiraUtil.Interfaces;
using UnityEngine;
using Zenject;
using CustomSabersLite.Components.Managers;
using CustomSabersLite.Utilities;

namespace CustomSabersLite.Components.Game;

internal class LiteSaberModelController : SaberModelController, IColorable, IPreSaberModelInit
{
    [Inject] private readonly LevelSaberManager levelSaberManager = null!;
    [Inject] private readonly TrailFactory trailFactory = null!;
    [Inject] private readonly SaberFactory saberFactory = null!;
    [Inject] private readonly EventManagerManager eventManagerManager = null!;
    [Inject] private readonly ColorManager colorManager = null!;
    [Inject] private readonly GameplayCoreSceneSetupData gameplaySetupData = null!;

    private LiteSaber? customSaberInstance;
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

        if (customSaberInstance == null)
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
        customTrailInstances?.ForEach(t => t.SetColor(color));
    }
}
