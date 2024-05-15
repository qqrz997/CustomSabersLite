using CustomSabersLite.Components.Interfaces;
using CustomSabersLite.Configuration;
using CustomSabersLite.Managers;
using CustomSabersLite.Utilities.AssetBundles;
using System.Threading.Tasks;

namespace CustomSabersLite.Components
{
    internal class LevelSaberManager
    {
        private readonly CSLConfig config;
        private readonly ISaberSet saberSet;
        private readonly SaberInstanceManager saberInstanceManager;
        private readonly ICustomSaberLoader saberLoader;

        public LevelSaberManager(CSLConfig config, ISaberSet saberSet, SaberInstanceManager saberInstanceManager, ICustomSaberLoader saberLoader)
        {
            this.config = config;
            this.saberSet = saberSet;
            this.saberInstanceManager = saberInstanceManager;
            this.saberLoader = saberLoader;
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
