using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CustomSabersLite.Services;
using SabersLib.Models;

namespace CustomSabersLite.Components;

internal class SaberInstanceTracker
{
    // temporary instance tracker to satisfy the SiraSaberFactory
    // if a mod requires a SiraSaber then CustomSabersLite will need to re-instantiate the saber prefab
    
    private readonly Stack<SaberInstanceSet> instances = [];
    private readonly SaberFactory saberFactory;

    public SaberInstanceTracker(SaberFactory saberFactory)
    {
        this.saberFactory = saberFactory;
    }

    public async Task<ITrailData[]> GetTrails(SaberType saberType)
    {
        var pop = await GetMostRecentSet();
        return saberType == SaberType.SaberA ? pop.LeftTrails : pop.RightTrails;
    }
    
    public async Task<ISaber?> GetSaber(SaberType saberType)
    {
        Logger.Info($"GetSaber {saberType}");
        var pop = await GetMostRecentSet();
        var saber = saberType == SaberType.SaberA ? pop.LeftSaber : pop.RightSaber;
        if (saber is null)
        {
            return null;
        }

        if (!saber.InUse)
        {
            saber.InUse = true;
            return saber;
        }

        var push = await saberFactory.InstantiateCurrentSabers(CancellationToken.None);
        instances.Push(push);
        
        saber = saberType == SaberType.SaberA ? push.LeftSaber : push.RightSaber;
        if (saber is null)
        {
            return null;
        }
        
        saber.InUse = true;
        return saber;
    }

    private async Task<SaberInstanceSet> GetMostRecentSet()
    {
        if (!instances.TryPeek(out var pop))
        {
            pop = await saberFactory.InstantiateCurrentSabers(CancellationToken.None);
            instances.Push(pop);
        }
        
        return pop;
    }
}