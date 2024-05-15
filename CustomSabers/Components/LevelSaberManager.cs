using CustomSabersLite.Components.Interfaces;
using CustomSabersLite.Configuration;
using System.Threading.Tasks;

namespace CustomSabersLite.Components
{
    internal class LevelSaberManager
    {
        private readonly CSLConfig config;
        private readonly ISaberSet saberSet;

        public LevelSaberManager(CSLConfig config, ISaberSet saberSet)
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
            await saberSet.SetSabers(config.CurrentlySelectedSaber);
    }
}
