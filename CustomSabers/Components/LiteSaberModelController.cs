using CustomSabersLite.Configuration;
using CustomSabersLite.Services;
using SabersCore.Components;
using SabersCore.Models;
using SabersCore.Services;
using SabersCore.Utilities.Common;
using SiraUtil.Interfaces;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Components;

internal class LiteSaberModelController : SaberModelController, IColorable, IPreSaberModelInit
{
    [Inject] private readonly PluginConfig config = null!;
    [Inject] private readonly GameplaySaberProvider gameplaySaberProvider = null!;
    [Inject] private readonly ITrailFactory trailFactory = null!;
    [Inject] private readonly ICustomSaberEventManagerHandler eventManagerHandler = null!;
    
    [Inject] private readonly ColorManager colorManager = null!;
    [Inject] private readonly GameplayCoreSceneSetupData gameplayCoreSceneSetupData = null!;
    [Inject] private readonly BeatmapCallbacksController beatmapCallbacksController = null!;
    
    private ISaber? saberInstance;
    private CustomSaberTrail[] customTrailInstances = [];
    private Color color;

    // Called by SiraUtil events
    public Color Color
    {
        get => color;
        set => SiraUtilSetColor(value);
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
        var sabers = await gameplaySaberProvider.GetSabers();
        saberInstance = sabers.GetSaberForType(saber.saberType);
        
        if (saberInstance is null)
        {
            Logger.Error("Something went wrong when getting the custom saber instance");
            return;
        }

        saberInstance.SetParent(transform);
        saberInstance.GameObject.SetActive(true);
        
        if (config.OverrideSaberLength)
        {
            saberInstance.SetLength(config.SaberLength);
        }

        if (config.OverrideSaberWidth)
        {
            saberInstance.SetWidth(config.SaberWidth);
        }

        if (config.EnableCustomEvents)
        {
            eventManagerHandler.InitializeEventManager(saberInstance.GameObject, saber.saberType);
        }

        customTrailInstances = trailFactory.AddTrailsTo(
            saberInstance,
            sabers.GetTrailsForType(saber.saberType),
            gameplayCoreSceneSetupData.playerSpecificSettings.saberTrailIntensity);
        
        customTrailInstances.ConfigureTrails(new(
            config.DisableWhiteTrail,
            config.OverrideTrailWidth,
            config.TrailWidth,
            config.OverrideTrailDuration,
            config.TrailDuration));

        saberInstance.SetColorScheme(colorManager._colorScheme);
        foreach (var trail in customTrailInstances)
        {
            trail.SetColorScheme(colorManager._colorScheme);
        }

        beatmapCallbacksController.AddBeatmapCallback<ColorBoostBeatmapEventData>(HandleColorBoostEvent);
    }

    private void HandleColorBoostEvent(ColorBoostBeatmapEventData eventData)
    {
        saberInstance?.SetBoostColors(colorManager._colorScheme, eventData.boostColorsAreOn);
        foreach (var trail in customTrailInstances)
        {
            trail.SetBoostColors(colorManager._colorScheme, eventData.boostColorsAreOn);
        }
    }
    
    public void SiraUtilSetColor(Color color)
    {
        this.color = color;
        saberInstance?.SetSpecificColor(color);
        foreach (var trail in customTrailInstances) trail.SetSpecificColor(color);
    }
}
