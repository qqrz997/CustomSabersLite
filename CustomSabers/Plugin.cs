using IPA;
using IPALogger = IPA.Logging.Logger;
using IPA.Config;
using IPA.Utilities;
using IPA.Config.Stores;
using Config = IPA.Config.Config;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using BS_Utils;
using CustomSaber.Configuration;
using CustomSaber.Utilities;
using BS_Utils.Utilities;
using System.IO;
using CustomSaber.UI;
using System.Reflection;

namespace CustomSaber
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }

        public static string PluginName => "Custom Sabers Lite";

        public static string PluginGUID => "qqrz.CustomSabersLite";

        public static string CustomSaberAssetsPath => Path.Combine(UnityGame.InstallPath, "CustomSabers");

        public static IPALogger Log { get; private set; }

        [Init]
        public void Init(IPALogger logger, Config config)
        {
            Log = logger;
            CustomSaberConfig.Instance = config.Generated<CustomSaberConfig>();
            Log.Debug("Config Loaded");
        }

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        /*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        */
        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            CustomSaberAssetLoader.Load();
            SettingsUI.CreateMenu();
            AddEvents();
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            //CustomSaberAssetLoader.Clear();
            RemoveEvents();
        }

        private void OnGameSceneLoaded()
        {
            SaberScript.Load();
        }

        private void AddEvents()
        {
            RemoveEvents();
            BSEvents.gameSceneLoaded += OnGameSceneLoaded;
        }

        private void RemoveEvents()
        {
            BSEvents.gameSceneLoaded -= OnGameSceneLoaded;
        }
    }
}
