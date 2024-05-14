using CustomSabersLite.Components.Interfaces;
using CustomSabersLite.Configuration;
using CustomSabersLite.Data;
using CustomSabersLite.Managers;
using CustomSabersLite.Utilities.AssetBundles;
using CustomSabersLite.Utilities.Extensions;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace CustomSabersLite.Components
{
    internal class CSLSaberSet : ISaberSet, IInitializable
    {
        private readonly CSLConfig config;
        private readonly ICustomSaberLoader saberLoader;
        private readonly SaberInstanceManager saberInstanceManager;

        public CSLSaberSet(CSLConfig config, CSLAssetLoader assetLoader, ICustomSaberLoader saberLoader, SaberInstanceManager saberInstanceManager)
        {
            this.config = config;
            this.saberLoader = saberLoader;
            this.saberInstanceManager = saberInstanceManager;
        }

        private CSLSaber LeftSaber = null;
        private CSLSaber RightSaber = null;

        public async void Initialize() => await SetSabers(config.CurrentlySelectedSaber);

        public CSLSaber CustomSaberForSaberType(SaberType saberType) =>
            saberType == SaberType.SaberA ? LeftSaber : RightSaber;

        public void SetSabers(GameObject sabersObject)
        {
            LeftSaber = FromSabersObject(sabersObject, "LeftSaber");
            RightSaber = FromSabersObject(sabersObject, "RightSaber");
        }

        public async Task SetSabers(string saberPath)
        {
            if (saberPath == "Default")
            {
                Logger.Info("Found default sabers on trying to create saberset instance");
                return;
            }

            if (!saberInstanceManager.TryGetSaber(saberPath, out CustomSaberData saber))
            {
                Logger.Info("Selected saber isn't loaded, attempting to load... " + saberPath);
                saber = await saberLoader.LoadCustomSaberAsync(saberPath);
                Logger.Info(saber.Descriptor.SaberName + " has been loaded");
                if (saber.FilePath == "Default")
                {
                    return;
                }
                saberInstanceManager.AddSaber(saber);
            }
            else
            {
                Logger.Info("Found instance of " + saber.Descriptor.SaberName);
            }

            SetSabers(saber.SabersObject);
        }

        private CSLSaber FromSabersObject(GameObject sabersObject, string saberName) =>
            sabersObject.transform.Find(saberName)?.gameObject.TryGetComponentOrDefault<CSLSaber>();
    }
}
