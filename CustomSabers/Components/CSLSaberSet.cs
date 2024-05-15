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

        public async void Initialize() => 
            await SetSabers(config.CurrentlySelectedSaber);

        public CSLSaber CustomSaberForSaberType(SaberType saberType) =>
            saberType == SaberType.SaberA ? LeftSaber : RightSaber;

        public async Task SetSabers(string saberPath)
        {
            if (saberPath == "Default") return;
            SetSabers(await GetSaberObject(saberPath));
        }

        public async Task InstantiateSabers(string saberPath)
        {
            if (saberPath == "Default") return;
            SetSabers(GameObject.Instantiate(await GetSaberObject(saberPath)));
        }

        public async Task<GameObject> GetSaberObject(string saberPath)
        {
            if (!saberInstanceManager.TryGetSaber(saberPath, out CustomSaberData saber))
            {
                saber = await saberLoader.LoadCustomSaberAsync(saberPath);
                if (saber.FilePath == "Default")
                {
                    return new GameObject("Invalid Saber");
                }
                saberInstanceManager.AddSaber(saber);
            }
            return saber.SabersObject;
        }

        private void SetSabers(GameObject sabersObject)
        {
            LeftSaber = FromSabersObject(sabersObject, "LeftSaber");
            RightSaber = FromSabersObject(sabersObject, "RightSaber");
        }

        private CSLSaber FromSabersObject(GameObject sabersObject, string saberName) =>
            sabersObject.transform.Find(saberName)?.gameObject.TryGetComponentOrDefault<CSLSaber>();
    }
}
