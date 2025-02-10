using System.Threading.Tasks;
using CustomSabersLite.Configuration;
using CustomSabersLite.Models;

namespace CustomSabersLite.Utilities.Services;

internal class SaberFactory
{
    private readonly CslConfig config;
    private readonly CustomSabersLoader customSabersLoader;
    private readonly GameResourcesProvider gameResourcesProvider;
    private readonly SaberMetadataCache saberMetadataCache;
    private readonly TrailFactory trailFactory;

    public SaberFactory(
        CslConfig config,
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

    public async Task<SaberInstanceSet> InstantiateCurrentSabers()
    {
        var selectedSaber = await GetCurrentSaberData();
        var (leftTrails, rightTrails) = await GetCurrentTrailData();

        return (selectedSaber.Prefab?.Instantiate() ?? CreateDefaultSaberSet()).WithTrails(leftTrails, rightTrails);
    }

    private async Task<(ITrailData[] leftTrails, ITrailData[] rightTrails)> GetCurrentTrailData()
    {
        if (config.CurrentlySelectedTrail is null)
        {
            return ([], []); // in this case, we will have no trails
        }

        var saberHash = config.CurrentlySelectedTrail switch
        {
            "custom" => config.CurrentlySelectedSaber,
            _ => config.CurrentlySelectedTrail
        };

        if (saberHash is null)
        {
            return GetDefaultTrailData();
        }

        var meta = saberMetadataCache.GetOrDefault(saberHash);

        if (meta is null || !meta.SaberFile.FileInfo.Exists)
        {
            return GetDefaultTrailData(); // the default trails will be used as a fallback
        }

        var saberData = await customSabersLoader.GetSaberData(meta.SaberFile, true);

        return saberData.Prefab is not null ? GetTrailsFromPrefab(saberData.Prefab) : GetDefaultTrailData();
    }

    private async Task<ISaberData> GetCurrentSaberData()
    {
        if (config.CurrentlySelectedSaber is null)
        {
            return NoSaberData.Value;
        }

        var meta = saberMetadataCache.GetOrDefault(config.CurrentlySelectedSaber);
        
        return meta is null || !meta.SaberFile.FileInfo.Exists ? NoSaberData.Value
            : await customSabersLoader.GetSaberData(meta.SaberFile, true);
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

    private static (ITrailData[], ITrailData[]) GetTrailsFromPrefab(ISaberPrefab prefab) =>
    (
        prefab.GetTrailsForType(SaberType.SaberA),
        prefab.GetTrailsForType(SaberType.SaberB)
    );
}
