using CustomSabersLite.Components.Managers;
using CustomSabersLite.Models;
using System.Threading.Tasks;
using Zenject;

namespace CustomSabersLite.Components.Game;

internal class LevelSaberManager : IInitializable
{
    private readonly SaberFactory saberFactory;

    public Task<ISaberData> SaberSetupTask { get; }

    public ISaberData CurrentSaberData { get; private set; }

    public LevelSaberManager(SaberFactory saberFactory)
    {
        this.saberFactory = saberFactory;
        SaberSetupTask = CreateLevelSaberInstance();
        CurrentSaberData = new NoSaberData();
    }

    public async void Initialize() =>
        await SaberSetupTask;

    private async Task<ISaberData> CreateLevelSaberInstance() =>
        CurrentSaberData = await saberFactory.GetCurrentSaberDataAsync();
}