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

        private static string PluginName => "Custom Sabers Lite";

        private static string PluginGUID => "qqrz997.CustomSabersLite";

        public const string Version = "0.6.0";

        internal static IPALogger Log { get; private set; }

        [Init]
        public async void Init(IPALogger logger, Config config, Zenjector zenjector)
        {
            Log = logger;

            CSLConfig.Instance = config.Generated<CSLConfig>();

            // PluginDirs.Init();

            if (!await CSLUtils.LoadCustomSaberAssembly())
            {
                return;
            }

            zenjector.UseLogger(logger);

            zenjector.Install<CSLAppInstaller>(Location.App, logger);
            zenjector.Install<CSLGameInstaller>(Location.Player);
            zenjector.Install<CSLMenuInstaller>(Location.Menu);
        }

        [OnStart]
        public void OnApplicationStart()
        {
            UIManager.CreateMenu();
            AddEvents();
            try
            {
                // await Task.WhenAll(CustomSaberAssetLoader.LoadAsync());
                // CustomSaberAssetLoader.Init();
            } 
            catch (Exception ex)
            {
                Log.Error("Issue encountered when loading custom sabers");
                Log.Error(ex);
            }
            UIManager.UpdateMenuOnSabersLoaded();
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            CSLAssetLoader.Clear();
            RemoveEvents();
        }

        private void OnGameSceneLoaded()
        {
            if (CSLAssetLoader.IsLoaded)
            {
                CSLSaberManager customSaberManager = new CSLSaberManager();
                customSaberManager.Init();
            }
        }

        private void OnMenuSceneLoaded()
        {
            // this doesn't actually refresh the button
            if (!UIManager.MenuButtonActive && CSLAssetLoader.IsLoaded) UIManager.UpdateMenu(true);
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
