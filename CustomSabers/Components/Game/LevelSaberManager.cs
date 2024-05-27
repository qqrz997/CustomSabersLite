using CustomSabersLite.Components.Managers;
using CustomSabersLite.Configuration;
using System.Threading.Tasks;

namespace CustomSabersLite.Components.Game
{
    internal class LevelSaberManager
    {
        private readonly CSLConfig config;
        private readonly LiteSaberSet saberSet;

        public LevelSaberManager(CSLConfig config, LiteSaberSet saberSet)
        {
            this.config = config;
            this.saberSet = saberSet;
            Initialize();
        }

        public Task SaberSetupTask { get; private set; }

        private async void Initialize()
        {
            SaberSetupTask = CreatLevelSaberInstance();
            await SaberSetupTask;
        }

        private async Task CreatLevelSaberInstance() =>
            await saberSet.InstantiateSabers(config.CurrentlySelectedSaber);
    }
}
