﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomSaber.Configuration;
using CustomSaber.Data;

namespace CustomSaber.Utilities
{
    public class CustomSaberAssetLoader
    {
        public static bool IsLoaded {  get; private set; }
        public static int SelectedSaber { get; internal set; } = 0;
        public static IList<CustomSaberData> CustomSaber { get; private set; } = new List<CustomSaberData>();
        public static IEnumerable<string> CustomSaberFiles { get; private set; } = Enumerable.Empty<string>();
        public static Action customSabersLoaded;

        internal static void Load()
        {
            Directory.CreateDirectory(Plugin.CustomSaberAssetsPath);

            IEnumerable<string> saberExt = new List<string> { "*.saber" };

            Plugin.Log.Debug($"Searching directory for external sabers: {Plugin.CustomSaberAssetsPath}");

            CustomSaberFiles = CustomSaberUtils.GetFileNames(Plugin.CustomSaberAssetsPath, saberExt, SearchOption.AllDirectories, true);

            Plugin.Log.Info($"{CustomSaberFiles.Count()} external sabers found.");

            CustomSaber = LoadCustomSaber(CustomSaberFiles);

            Plugin.Log.Info($"{CustomSaber.Count} total sabers loaded");

            Plugin.Log.Debug($"Currently selected saber: {CustomSaberConfig.Instance.CurrentlySelectedSaber}");

            if (CustomSaberConfig.Instance.CurrentlySelectedSaber != null)
            {
                for (int i = 0; i < CustomSaber.Count; i++)
                {
                    if (CustomSaber[i].FileName == CustomSaberConfig.Instance.CurrentlySelectedSaber)
                    {
                        SelectedSaber = i;
                        break;
                    }
                }
            }

            IsLoaded = true;
            customSabersLoaded?.Invoke();
        }

        internal static void Reload()
        {
            Plugin.Log.Debug("Reloading the CustomSaberAssetLoader");
            Clear();
            Load();
        }

        internal static void Clear()
        {
            int numberOfObjects = CustomSaber.Count;
            for (int i = 0; i < numberOfObjects; i++)
            {
                CustomSaber[i].Destroy();
                CustomSaber[i] = null;
            }

            SelectedSaber = 0;
            CustomSaber = new List<CustomSaberData>();
            CustomSaberFiles = Enumerable.Empty<string>();
        }

        private static IList<CustomSaberData> LoadCustomSaber(IEnumerable<string> customSaberFiles)
        {
            IList<CustomSaberData> customSabers = new List<CustomSaberData>
            {
                new CustomSaberData("DefaultSabers"),
            };
            
            foreach (string customSaberFile in customSaberFiles)
            {
                try
                {
                    CustomSaberData newSaber = new CustomSaberData(customSaberFile);
                    if (newSaber != null)
                    {
                        customSabers.Add(newSaber);
                    }
                }
                catch (Exception ex)
                {
                    Plugin.Log.Error($"Failed to load custom saber with name {customSaberFile}");
                    Plugin.Log.Error(ex);
                }
            }

            return customSabers;
        }
        
        //public static int DeleteCurrentSaber(){}
    }
}
