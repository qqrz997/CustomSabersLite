using CustomSabersLite.Models;
using System.Threading.Tasks;

namespace CustomSabersLite.Utilities.Services;

internal class GameplaySaber
{
    private readonly SaberFactory saberFactory;

    public Task<ISaberData> CreateTask { get; }

    public GameplaySaber(SaberFactory saberFactory)
    {
        this.saberFactory = saberFactory;
        CreateTask = CreateLevelSaberInstance();
    }

    private async Task<ISaberData> CreateLevelSaberInstance()
    {
        return await saberFactory.GetCurrentSaberDataAsync();
    }
}