using CustomSabersLite.Components.Managers;
using CustomSabersLite.Configuration;
using System.Threading.Tasks;
using Zenject;

namespace CustomSabersLite.Components.Game;

internal class LevelSaberManager : IInitializable
{
    private readonly CSLConfig config;
    private readonly LiteSaberSet saberSet;

    public Task SaberSetupTask { get; }

    public LevelSaberManager(CSLConfig config, LiteSaberSet saberSet)
    {
        this.config = config;
        this.saberSet = saberSet;
        SaberSetupTask = CreateLevelSaberInstance();
    }

    public async void Initialize() => 
        await SaberSetupTask;

    private async Task CreateLevelSaberInstance() => 
        await saberSet.SetSabers(config.CurrentlySelectedSaber);
}
