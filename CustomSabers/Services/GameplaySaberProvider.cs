using System.Threading;
using System.Threading.Tasks;
using SabersCore.Models;

namespace CustomSabersLite.Services;

internal class GameplaySaberProvider
{
    private readonly SaberFactory saberFactory;
    
    public GameplaySaberProvider(SaberFactory saberFactory)
    {
        this.saberFactory = saberFactory;
    }

    private Task<SaberInstanceSet>? saberInstance;

    public async Task<SaberInstanceSet> GetSabers()
    {
        saberInstance ??= saberFactory.InstantiateCurrentSabers(CancellationToken.None);
        return await saberInstance;
    }
}