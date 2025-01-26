using System.Threading.Tasks;
using CustomSabersLite.Configuration;
using SiraUtil.Interfaces;
using UnityEngine;
using Zenject;
using CustomSabersLite.Utilities.Services;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Extensions;

namespace CustomSabersLite.Components;

internal class LiteSaberModelController : SaberModelController, IColorable, IPreSaberModelInit
{
    [Inject] private readonly CslConfig config = null!;
    [Inject] private readonly Task<SaberInstanceSet> saberInstanceSet = null!;
    [Inject] private readonly TrailFactory trailFactory = null!;
    [Inject] private readonly SaberEventService saberEventService = null!;
    
    [Inject] private readonly ColorManager colorManager = null!;
    [Inject] private readonly GameplayCoreSceneSetupData gameplaySetupData = null!;
    
    private ILiteSaber? liteSaberInstance;
    private LiteSaberTrail[] customTrailInstances = [];

    private Color color;
    public Color Color
    {
        get => color; 
        set
        {
            color = value;
            liteSaberInstance?.SetColor(value);
            customTrailInstances.ForEach(t => t.SetColor(value));
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

        liteSaberInstance = (await saberInstanceSet).GetSaberForType(saber.saberType);

        if (liteSaberInstance == null)
        {
            Logger.Error("Something went wrong when getting the custom saber instance");
            return;
        }

        liteSaberInstance.SetParent(transform);
        if (config.OverrideSaberLength) liteSaberInstance.SetLength(config.SaberLength);
        if (config.OverrideSaberWidth) liteSaberInstance.SetWidth(config.SaberWidth);
        if (config.EnableCustomEvents) saberEventService.InitializeEventManager(liteSaberInstance.EventManager, saber.saberType);
        
        float intensity = gameplaySetupData.playerSpecificSettings.saberTrailIntensity;
        customTrailInstances = trailFactory.CreateTrail(liteSaberInstance, saber.saberType, intensity);

        Color = colorManager.ColorForSaberType(saber.saberType);
    }
}
