using CustomSabersLite.Models;
using System.Threading.Tasks;

namespace CustomSabersLite.Utilities.Services;

internal class LevelSaberManager
{
    private readonly SaberFactory saberFactory;

    public Task<ISaberData> LevelSaberInstance { get; }

    public LevelSaberManager(SaberFactory saberFactory)
    {
        this.saberFactory = saberFactory;
        LevelSaberInstance = CreateLevelSaberInstance();
    }

    private async Task<ISaberData> CreateLevelSaberInstance()
    {
        return await saberFactory.GetCurrentSaberDataAsync();
    }
}