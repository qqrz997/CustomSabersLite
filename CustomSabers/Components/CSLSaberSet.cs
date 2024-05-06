using CustomSabersLite.Components.Interfaces;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities.AssetBundles;
using System;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Components
{
    internal class CSLSaberSet : ISaberSet, IInitializable, IDisposable
    {
        private readonly CSLConfig config;
        private readonly CSLAssetLoader assetLoader;

        public CSLSaberSet(CSLConfig config, CSLAssetLoader assetLoader)
        {
            this.config = config;
            this.assetLoader = assetLoader;
        }

        private GameObject sabers = null;
        private CSLSaber LeftSaber = null;
        private CSLSaber RightSaber = null;

        public void Initialize()
        {
            CreateCustomSaberInstance(assetLoader.SelectedSaber);
        }

        public void Dispose() => DestroySabers();

        public CSLSaber CustomSaberForSaberType(SaberType saberType)
        {
            return GetSaberInstance(saberType);
        }

        private CSLSaber GetSaberInstance(SaberType saberType)
        {
            // Check if sabers are loaded or have changed
            if (!sabers || config.CurrentlySelectedSaber != assetLoader.SelectedSaber.FilePath)
            {
                LoadSabers();
            }
            if (saberType == SaberType.SaberA)
                return LeftSaber;
            else
                return RightSaber;
        }

        private void LoadSabers()
        {
            DestroySabers();
            Logger.Debug("Sabers loading..."); // assetLoader.SelectedSaber > previously selected saber

            if (config.CurrentlySelectedSaber == "Default" || config.CurrentlySelectedSaber == null)
            {
                // Use default sabers
                assetLoader.SelectedSaber = new CustomSaberData("Default");
            }
            else
            {
                // Use custom sabers
                assetLoader.SelectedSaber = assetLoader.LoadSaberWithRepair(config.CurrentlySelectedSaber);
                CreateCustomSaberInstance(assetLoader.SelectedSaber);
            }
        }

        private void CreateCustomSaberInstance(CustomSaberData customSaberData)
        {
            Logger.Debug("Setting custom saber instance...");
            if (customSaberData is null)
            {
                Logger.Critical("The data for the current selected saber is missing");
                // Saber instance not being set will break the mod - load a fallback saber?
                return;
            }

            if (customSaberData.FilePath != "Default")
            {
                sabers = GameObject.Instantiate(customSaberData.SabersObject);
                LeftSaber = sabers.transform.Find("LeftSaber").gameObject.AddComponent<CSLSaber>();
                RightSaber = sabers.transform.Find("RightSaber").gameObject.AddComponent<CSLSaber>();
            }
        }

        private void DestroySabers()
        {
            if (sabers)
            {
                assetLoader.SelectedSaber?.Destroy();
                GameObject.Destroy(sabers);
                GameObject.Destroy(LeftSaber);
                GameObject.Destroy(RightSaber);
            }
        }
    }
}
