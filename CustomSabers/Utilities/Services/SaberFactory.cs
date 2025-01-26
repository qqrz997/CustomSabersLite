using System.Threading.Tasks;
using CustomSabersLite.Configuration;
using CustomSabersLite.Models;

namespace CustomSabersLite.Utilities.Services;

internal class SaberFactory(CustomSabersLoader customSabersLoader, GameResourcesProvider gameResourcesProvider, CslConfig config)
{
    private readonly CustomSabersLoader customSabersLoader = customSabersLoader;
    private readonly CslConfig config = config;
    private readonly GameResourcesProvider gameResourcesProvider = gameResourcesProvider;

    public async Task<SaberInstanceSet> InstantiateCurrentSabers() => 
        (await GetCurrentSaberDataAsync()).Prefab?.Instantiate() ?? CreateDefaultSaberSet();
    
    private async Task<ISaberData> GetCurrentSaberDataAsync() =>
        config.CurrentlySelectedSaber is null or [] ? NoSaberData.Value
        : await customSabersLoader.GetSaberData(config.CurrentlySelectedSaber, true);

    private SaberInstanceSet CreateDefaultSaberSet() =>
        new(new DefaultSaber(gameResourcesProvider.CreateNewDefaultSaber()), 
            new DefaultSaber(gameResourcesProvider.CreateNewDefaultSaber()));
}
