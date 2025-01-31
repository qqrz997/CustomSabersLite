using System.Threading.Tasks;
using CustomSabersLite.Configuration;
using CustomSabersLite.Models;

namespace CustomSabersLite.Utilities.Services;

internal class SaberFactory
{
    private readonly CustomSabersLoader customSabersLoader;
    private readonly CslConfig config;
    private readonly GameResourcesProvider gameResourcesProvider;
    private readonly SaberMetadataCache saberMetadataCache;

    public SaberFactory(CustomSabersLoader customSabersLoader, GameResourcesProvider gameResourcesProvider, CslConfig config, SaberMetadataCache saberMetadataCache)
    {
        this.customSabersLoader = customSabersLoader;
        this.config = config;
        this.saberMetadataCache = saberMetadataCache;
        this.gameResourcesProvider = gameResourcesProvider;
    }

    public async Task<SaberInstanceSet> InstantiateCurrentSabers() => 
        (await GetCurrentSaberDataAsync()).Prefab?.Instantiate() ?? CreateDefaultSaberSet();
    
    private async Task<ISaberData> GetCurrentSaberDataAsync()
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
            new DefaultSaber(gameResourcesProvider.CreateNewDefaultSaber()));
}
