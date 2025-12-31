using System.Threading;
using System.Threading.Tasks;
using CustomSabersLite.Configuration;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Extensions;
using SabersCore.Models;
using SabersCore.Services;

namespace CustomSabersLite.Services;

internal class SaberFactory
{
    private readonly PluginConfig config;
    private readonly ISaberInstanceFactory saberInstanceFactory;

    public SaberFactory(
        PluginConfig config,
        ISaberInstanceFactory saberInstanceFactory)
    {
        this.config = config;
        this.saberInstanceFactory = saberInstanceFactory;
    }

    public async Task<SaberInstanceSet> InstantiateCurrentSabers(CancellationToken token)
    {
        Logger.Debug("Creating current sabers");

        config.CurrentlySelectedSaber.TryGetSaberHash(out var saberHash);
        var sabers = await saberInstanceFactory.CreateSaberSet(saberHash?.Hash, token);
        
        return config.CurrentlySelectedTrail switch
        {
            CustomTrailValue => sabers,
            NoTrailValue => sabers.WithTrails([], []),
            SaberHash trailHash => await saberInstanceFactory.ReplaceTrailsWithOther(sabers, trailHash.Hash, token),
            _ => await saberInstanceFactory.ReplaceTrailsWithOther(sabers, null, token)
        };
    }
}
