using IPA;
using IPALogger = IPA.Logging.Logger;
using IPA.Config.Stores;
using Config = IPA.Config.Config;
using CustomSabersLite.Configuration;
using CustomSabersLite.Utilities;
using BS_Utils.Utilities;
using CustomSabersLite.UI;
using System;
using CustomSabersLite.Components;
using SiraUtil.Zenject;
using CustomSabersLite.Installers;


namespace CustomSabersLite
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    internal class Plugin
    {
        internal static Plugin Instance { get; private set; }

        public const string Version = "0.6.0";

        [Init]
        public async void Init(IPALogger logger, Config config, Zenjector zenjector)
        {
            Logger.Log = logger;
            
            CSLConfig pluginConfig = config.Generated<CSLConfig>();

            if (!await CSLUtils.LoadCustomSaberAssembly())
            {
                return;
            }

            zenjector.UseLogger(logger);

            zenjector.Install<CSLAppInstaller>(Location.App, logger, pluginConfig);
            zenjector.Install<CSLMenuInstaller>(Location.Menu);
            zenjector.Install<CSLGameInstaller>(Location.Player);
        }

        [OnStart]
        public void OnApplicationStart()
        {
            // UIManager.CreateMenu();
            AddEvents();
            try
            {
                // await Task.WhenAll(CustomSaberAssetLoader.LoadAsync());
                // CustomSaberAssetLoader.Init();
            } 
            catch (Exception ex)
            {
                Logger.Error("Issue encountered when loading custom sabers");
                Logger.Error(ex.ToString());
            }
            // UIManager.UpdateMenuOnSabersLoaded();
        }

        private CSLSaberManager saberManager;

        [OnExit]
        public void OnApplicationQuit()
        {
            // CSLAssetLoader.Clear();
            RemoveEvents();
        }

        private void OnGameSceneLoaded()
        {
            /*Logger.Info("game scene loaded");
            saberManager = new CSLSaberManager();
            saberManager.Init();*/
        }

        private void OnMenuSceneLoaded()
        {

        }

        private void AddEvents()
        {
            RemoveEvents();
            BSEvents.gameSceneLoaded += OnGameSceneLoaded;
            BSEvents.menuSceneActive += OnMenuSceneLoaded;
        }

        private void RemoveEvents()
        {
            BSEvents.gameSceneLoaded -= OnGameSceneLoaded;
            BSEvents.menuSceneActive -= OnMenuSceneLoaded;
        }
    }
}
