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

    public ILiteSaber Create(SaberType saberType, ISaberData saberData) => saberData switch
    {
        CustomSaberData => CreateCustomLiteSaber(saberType, saberData),
        NoSaberData => CreateDefaultLiteSaber(saberType),
            
        // throw to indicate there is a mistake in the model
        _ => throw new ArgumentOutOfRangeException(nameof(saberData))
    };

    private ILiteSaber CreateCustomLiteSaber(SaberType saberType, ISaberData saberData)
    {
        var prefab = saberData.GetPrefab(saberType);

        return prefab == null ? CreateDefaultLiteSaber(saberType)
            : new CustomLiteSaber(Instantiate(prefab), saberData.Metadata.SaberFile.Type);
    }

    private DefaultSaber CreateDefaultLiteSaber(SaberType saberType)
    {
        if (defaultSaberPrefab == null)
        {
            throw new NullReferenceException(nameof(defaultSaberPrefab));
        }

        return saberType switch
        {
            SaberType.SaberA => new DefaultSaber(Instantiate(defaultSaberPrefab)),
            SaberType.SaberB => new DefaultSaber(Instantiate(defaultSaberPrefab)),
            _ => throw new ArgumentOutOfRangeException(nameof(saberType))
        };
    }
}
