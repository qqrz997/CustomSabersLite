﻿using System.Threading;
using System.Threading.Tasks;
using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Extensions;

namespace CustomSabersLite.Services;

internal class SaberFactory
{
    private readonly PluginConfig config;
    private readonly CustomSabersLoader customSabersLoader;
    private readonly GameResourcesProvider gameResourcesProvider;
    private readonly SaberMetadataCache saberMetadataCache;
    private readonly TrailFactory trailFactory;

    public SaberFactory(
        PluginConfig config,
        CustomSabersLoader customSabersLoader,
        GameResourcesProvider gameResourcesProvider,
        SaberMetadataCache saberMetadataCache,
        TrailFactory trailFactory)
    {
        this.config = config;
        this.customSabersLoader = customSabersLoader;
        this.gameResourcesProvider = gameResourcesProvider;
        this.saberMetadataCache = saberMetadataCache;
        this.trailFactory = trailFactory;
    }

    public async Task<SaberInstanceSet> InstantiateCurrentSabers(CancellationToken token)
    {
        Logger.Debug("Creating a new saber set");
        
        var selectedSaber = 
            !config.CurrentlySelectedSaber.TryGetSaberHash(out var saberHash) ? null 
            : !saberMetadataCache.TryGetMetadata(saberHash.Hash, out var meta) ? null
            : await customSabersLoader.GetSaberData(meta.SaberFile, true, token);
        
        var (leftTrails, rightTrails) = config.CurrentlySelectedTrail switch
        {
            NoTrailValue => ([], []),
            CustomTrailValue => GetTrailsFromPrefab(selectedSaber?.Prefab),
            SaberHash trailHash => await LoadTrailsFromSaber(trailHash, token),
            _ => GetDefaultTrailData(),
        };

        return (selectedSaber?.Prefab?.Instantiate() ?? CreateDefaultSaberSet()).WithTrails(leftTrails, rightTrails);
    }

    private async Task<(ITrailData[] leftTrails, ITrailData[] rightTrails)> LoadTrailsFromSaber(
        SaberHash saberHash, CancellationToken token)
    {
        if (!saberMetadataCache.TryGetMetadata(saberHash.Hash, out var meta) || !meta.SaberFile.FileInfo.Exists)
        {
            return GetDefaultTrailData(); // the default trails will be used as a fallback
        }

        var saberData = await customSabersLoader.GetSaberData(meta.SaberFile, true, token);

        return saberData.Prefab is null ? GetDefaultTrailData() 
            : (saberData.Prefab.GetTrailsForType(SaberType.SaberA), 
                saberData.Prefab.GetTrailsForType(SaberType.SaberB));
    }

    private SaberInstanceSet CreateDefaultSaberSet() =>
        new(new DefaultSaber(gameResourcesProvider.CreateNewDefaultSaber()),
            new DefaultSaber(gameResourcesProvider.CreateNewDefaultSaber()),
            [trailFactory.CreateDefaultTrailData()],
            [trailFactory.CreateDefaultTrailData()]);

    private (ITrailData[], ITrailData[]) GetDefaultTrailData() => 
    (
        [trailFactory.CreateDefaultTrailData()],
        [trailFactory.CreateDefaultTrailData()]
    );

    private (ITrailData[], ITrailData[]) GetTrailsFromPrefab(ISaberPrefab? prefab) =>
        prefab is null ? GetDefaultTrailData()
        : (prefab.GetTrailsForType(SaberType.SaberA), prefab.GetTrailsForType(SaberType.SaberB));
}
