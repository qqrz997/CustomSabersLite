﻿using CustomSabersLite.Components.Managers;
using CustomSabersLite.Models;
using System.Threading.Tasks;
using Zenject;

namespace CustomSabersLite.Components.Game;

internal class LevelSaberManager : IInitializable
{
    private readonly SaberFactory saberFactory;

    public Task<CustomSaberData> SaberSetupTask { get; }

    public CustomSaberData CurrentSaberData { get; private set; }

    public LevelSaberManager(SaberFactory saberFactory)
    {
        this.saberFactory = saberFactory;
        SaberSetupTask = CreateLevelSaberInstance();
    }

    public async void Initialize() =>
        await SaberSetupTask;

    private async Task<CustomSaberData> CreateLevelSaberInstance() =>
        CurrentSaberData = await saberFactory.GetCurrentSaberDataAsync();
}