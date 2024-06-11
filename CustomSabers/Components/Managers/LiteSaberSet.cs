using CustomSabersLite.Components.Game;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities.AssetBundles;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.Components.Managers
{
    internal class LiteSaberSet : IDisposable
    {
        private readonly SaberInstanceManager saberInstanceManager;
        private readonly CustomSabersLoader customSabersLoader;

        public LiteSaberSet(SaberInstanceManager saberInstanceManager, CustomSabersLoader customSabersLoader)
        {
            this.saberInstanceManager = saberInstanceManager;
            this.customSabersLoader = customSabersLoader;
        }

        private LiteSaber LeftSaber = null;
        private LiteSaber RightSaber = null;

        public CustomSaberData Data { get; private set; }

        public LiteSaber CustomSaberForSaberType(SaberType saberType) =>
            saberType == SaberType.SaberA ? LeftSaber : RightSaber;

        public async Task SetSabers(string saberPath)
        {
            DestroySabers();

            CustomSaberData customSaberData = await GetSaberData(saberPath);
            if (customSaberData.SaberPrefab != null)
            {
                SetSabersFromPrefab(customSaberData.SaberPrefab);
            }
        }

        public async Task<CustomSaberData> GetSaberData(string saberPath)
        {
            Data = await customSabersLoader.GetSaberData(saberPath);
            return Data;
        }

        public void Dispose() => DestroySabers();

        public void DestroySabers()
        {
            if (LeftSaber)
            {
                GameObject.Destroy(LeftSaber.gameObject);
                LeftSaber = null;
            }
            if (RightSaber)
            {
                GameObject.Destroy(RightSaber.gameObject);
                RightSaber = null;
            }
        }

        private void SetSabersFromPrefab(GameObject saberPrefab)
        {
            LeftSaber = InstantiateSaber(SaberFromPrefab(saberPrefab, "LeftSaber"));
            RightSaber = InstantiateSaber(SaberFromPrefab(saberPrefab, "RightSaber"));
        }

        private LiteSaber InstantiateSaber(GameObject original) =>
            GameObject.Instantiate(original).AddComponent<LiteSaber>();

        private GameObject SaberFromPrefab(GameObject saberPrefab, string saberName) =>
            saberPrefab.transform.Find(saberName)?.gameObject;
    }
}
