using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using System;
using System.Threading.Tasks;
using UnityEngine;

using static UnityEngine.Object;

namespace CustomSabersLite.Utilities.Services;

internal class SaberFactory(CustomSabersLoader customSabersLoader, GameResourcesProvider gameResourcesProvider, CSLConfig config)
{
    private readonly CustomSabersLoader customSabersLoader = customSabersLoader;
    private readonly CSLConfig config = config;
    private readonly GameObject defaultSaberPrefab = gameResourcesProvider.SaberModelPrefab;

    public async Task<ISaberData> GetCurrentSaberDataAsync() =>
        config.CurrentlySelectedSaber is null ? NoSaberData.Value
        : await customSabersLoader.GetSaberData(config.CurrentlySelectedSaber, true);

    public ILiteSaber Create(SaberType saberType, ISaberData saberData) => 
        saberData is CustomSaberData && saberData.GetPrefab(saberType) is GameObject prefab 
            ? CustomLiteSaber.Create(prefab, saberData.Metadata.SaberFile.Type) 
            : DefaultSaber.Create(defaultSaberPrefab); 
}
