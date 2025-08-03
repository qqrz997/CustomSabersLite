using System.Threading.Tasks;
using CustomSabersLite.Configuration;
using CustomSabersLite.Services;
using CustomSabersLite.Utilities.Extensions;
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
    [Inject] private readonly Task<SaberInstanceSet> saberSet = null!;
    [Inject] private readonly ITrailFactory trailFactory = null!;
    [Inject] private readonly ICustomSaberEventManagerHandler eventManagerHandler = null!;
    
    [Inject] private readonly ColorManager colorManager = null!;
    [Inject] private readonly GameplayCoreSceneSetupData gameplayCoreSceneSetupData = null!;
    
    private ISaber? saberInstance;
    private CustomSaberTrail[] customTrailInstances = [];
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
        var sabers = await saberSet;
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

        SetColor(colorManager.ColorForSaberType(saber.saberType));
    }

    public void SetColor(Color color)
    {
        this.color = color;
        saberInstance?.SetColor(color);
        customTrailInstances.ForEach(t => t.SetColor(color));
    }
}
