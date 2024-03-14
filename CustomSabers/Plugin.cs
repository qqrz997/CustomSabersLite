using IPA;
using IPALogger = IPA.Logging.Logger;
using IPA.Config.Stores;
using Config = IPA.Config.Config;
using CustomSaber.Configuration;
using CustomSaber.Utilities;
using BS_Utils.Utilities;
using CustomSaber.UI;
using System;
using CustomSaber.Components;
// using SiraUtil.Zenject;
// using CustomSaber.Installers;

namespace CustomSaber
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
        public void Init(IPALogger logger, Config config/*, Zenjector zenjector*/)
        {
            Log = logger;

            CustomSaberConfig.Instance = config.Generated<CustomSaberConfig>();

            PluginDirs.Init();

            /*zenjector.Install(Location.App, Container => Container.BindInstance(config).AsSingle());
            zenjector.Install<CustomSabersMenuInstaller>(Location.Menu);*/
        }

        [OnStart]
        public void OnApplicationStart()
        {
            UIManager.CreateMenu();
            AddEvents();
            try
            {
                // await Task.WhenAll(CustomSaberAssetLoader.LoadAsync());
                CustomSaberAssetLoader.Init();
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
            CustomSaberAssetLoader.Clear();
            RemoveEvents();
        }

        private void OnGameSceneLoaded()
        {
            if (CustomSaberAssetLoader.IsLoaded)
            {
                CustomSaberManager customSaberManager = new CustomSaberManager();
                customSaberManager.Init();
            }
        }

        private void OnMenuSceneLoaded()
        {
            // this doesn't actually refresh the button
            if (!UIManager.MenuButtonActive && CustomSaberAssetLoader.IsLoaded) UIManager.UpdateMenu(true);
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
