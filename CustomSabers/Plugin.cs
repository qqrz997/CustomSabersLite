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

namespace CustomSaber
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    internal class Plugin
    {
        internal static Plugin Instance { get; private set; }

        private static string PluginName => "Custom Sabers Lite";

        private static string PluginGUID => "qqrz997.CustomSabersLite";

        public const string Version = "0.5.1";

        internal static IPALogger Log { get; private set; }

        [Init]
        public void Init(IPALogger logger, Config config)
        {
            Log = logger;
            CustomSaberConfig.Instance = config.Generated<CustomSaberConfig>();
            Log.Debug("Config Loaded");
            
            PluginDirs.Init();
        }

        [OnStart]
        public void OnApplicationStart()
        {
            SettingsUI.CreateMenu();
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
            SettingsUI.UpdateMenuOnSabersLoaded();
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
            if (!SettingsUI.MenuButtonActive && CustomSaberAssetLoader.IsLoaded) SettingsUI.UpdateMenu(true);
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
