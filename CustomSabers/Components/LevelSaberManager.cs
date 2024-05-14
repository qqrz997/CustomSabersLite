using CustomSabersLite.Components.Interfaces;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using CustomSabersLite.Managers;
using CustomSabersLite.Utilities.AssetBundles;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Components
{
    internal class LevelSaberManager : IInitializable
    {
        private readonly CSLConfig config;
        private readonly SaberInstanceManager saberInstanceManager;
        private readonly ICustomSaberLoader saberLoader;
        private readonly ISaberSet saberSet;

        public LevelSaberManager(CSLConfig config, SaberInstanceManager saberInstanceManager, ICustomSaberLoader saberLoader, ISaberSet saberSet)
        {
            this.config = config;
            this.saberInstanceManager = saberInstanceManager;
            this.saberLoader = saberLoader;
            this.saberSet = saberSet;
        }

        public Task SaberSetupTask { get; private set; }

        public async void Initialize()
        {
            SaberSetupTask = CreatLevelSaberInstance();
            Stopwatch sw = Stopwatch.StartNew();
            await SaberSetupTask;
            sw.Stop();
            Logger.Info($"Setup task completed in {sw.ElapsedMilliseconds}ms");
        }

        private async Task CreatLevelSaberInstance()
        {
            if (!saberInstanceManager.TryGetSaber(config.CurrentlySelectedSaber, out CustomSaberData saber))
            {
                saber = await saberLoader.LoadCustomSaberAsync(config.CurrentlySelectedSaber);
                if (saber.FilePath == "Default") return;
                saberInstanceManager.AddSaber(saber);
            }
            saberSet.SetSabers(GameObject.Instantiate(saber.SabersObject));
        }
    }
}
