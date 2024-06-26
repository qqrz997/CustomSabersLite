﻿using SiraUtil.Interfaces;
using UnityEngine;
using Zenject;
using CustomSabersLite.Configuration;
using CustomSabersLite.Components.Managers;

namespace CustomSabersLite.Components.Game;

internal class LiteSaberModelController : SaberModelController, IColorable, IPreSaberModelInit
{
    private TrailManager trailManager;
    private ColorManager colorManager;
    private SaberFactory saberFactory;
    private EventManagerManager eventManagerManager;
    private CSLConfig config;
    private LevelSaberManager levelSaberManager;

    [Inject]
    public void Construct(TrailManager trailManager, ColorManager colorManager, SaberFactory saberFactory, EventManagerManager eventManagerManager, CSLConfig config, LevelSaberManager levelSaberManager)
    {
        this.trailManager = trailManager;
        this.colorManager = colorManager;
        this.saberFactory = saberFactory;
        this.eventManagerManager = eventManagerManager;
        this.config = config;
        this.levelSaberManager = levelSaberManager;
    }

    private Color? color;

    public LiteSaber customSaberInstance;

    public LiteSaberTrail[] customTrailInstances;

    public Color Color { get => color.GetValueOrDefault(); set => SetColor(value); }

    public bool PreInit(Transform parent, Saber saber)
    {
        CustomSaberInit(parent, saber);
        return true;
    }

    private async void CustomSaberInit(Transform parent, Saber saber)
    {
        await levelSaberManager.SaberSetupTask;
        customSaberInstance = saberFactory.TryCreate(saber.saberType, levelSaberManager.CurrentSaberData);

        if (!customSaberInstance)
        {
            Logger.Error("Something went wrong when getting the custom saber instance");
            return;
        }

        transform.SetParent(parent, false);

        customSaberInstance.SetParent(transform);
        eventManagerManager.InitializeEventManager(customSaberInstance.EventManager, saber.saberType);

        var saberColor = config.EnableCustomColorScheme
            ? CustomSchemeColorForSaberType(saber.saberType)
            : colorManager.ColorForSaberType(saber.saberType);

        customTrailInstances = trailManager.CreateTrail(
            saber,
            _saberTrail,
            saberColor,
            customSaberInstance);

        SetColor(saberColor);
    }

    private Color CustomSchemeColorForSaberType(SaberType saberType) =>
        saberType == SaberType.SaberA ? config.LeftSaberColor : config.RightSaberColor;

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
