using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using CustomSabersLite.Services;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using SiraUtil.Interfaces;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Components;

internal class LiteSaberModelController : SaberModelController, IColorable, IPreSaberModelInit
{
    [Inject] private readonly PluginConfig config = null!;
    // [Inject] private readonly Task<SaberInstanceSet> saberInstanceSet = null!;
    [Inject] private readonly SaberInstanceTracker saberInstanceTracker = null!;
    [Inject] private readonly TrailFactory trailFactory = null!;
    [Inject] private readonly SaberEventService saberEventService = null!;
    
    [Inject] private readonly ColorManager colorManager = null!;
    [Inject] private readonly GameplayCoreSceneSetupData gameplayCoreSceneSetupData = null!;
    
    private ILiteSaber? liteSaberInstance;
    private LiteSaberTrail[] customTrailInstances = [];
    private Color color;

    public Color Color
    {
        get => color;
        set => SetColor(value);
    }

    public bool PreInit(Transform parent, Saber saber)
    {
        transform.SetParent(parent, false);
        transform.position = parent.position;
        transform.rotation = parent.rotation;
        
        CustomSaberInit(saber);
        return false;
    }

    private async void CustomSaberInit(Saber saber)
    {
        liteSaberInstance = await saberInstanceTracker.GetSaber(saber.saberType);

        if (liteSaberInstance is null)
        {
            Logger.Error("Something went wrong when getting the custom saber instance");
            return;
        }

        liteSaberInstance.SetParent(transform);
        
        if (config.OverrideSaberLength)
        {
            liteSaberInstance.SetLength(config.SaberLength);
        }

        if (config.OverrideSaberWidth)
        {
            liteSaberInstance.SetWidth(config.SaberWidth);
        }

        if (config.EnableCustomEvents)
        {
            saberEventService.InitializeEventManager(liteSaberInstance.EventManager, saber.saberType);
        }

        customTrailInstances = trailFactory.AddTrailsTo(
            liteSaberInstance,
            await saberInstanceTracker.GetTrails(saber.saberType),
            gameplayCoreSceneSetupData.playerSpecificSettings.saberTrailIntensity);
        
        customTrailInstances.ConfigureTrails(config);

        SetColor(colorManager.ColorForSaberType(saber.saberType));
    }

    public void SetColor(Color color)
    {
        this.color = color;
        liteSaberInstance?.SetColor(color);
        customTrailInstances.ForEach(t => t.SetColor(color));
    }
}
