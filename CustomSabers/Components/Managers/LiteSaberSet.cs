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
        private readonly WhackerLoader whackerLoader;
        private readonly CustomSaberLoader saberLoader;
        private readonly SaberInstanceManager saberInstanceManager;

        public LiteSaberSet(SaberInstanceManager saberInstanceManager, WhackerLoader whackerLoader, CustomSaberLoader saberLoader)
        {
            this.saberLoader = saberLoader;
            this.whackerLoader = whackerLoader;
            this.saberInstanceManager = saberInstanceManager;
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
            if (!saberInstanceManager.TryGetSaber(saberPath, out CustomSaberData saber))
            {
                switch (Path.GetExtension(saberPath))
                {
                    case FileExts.Saber:
                        saber = await saberLoader.LoadCustomSaberAsync(saberPath); break;

                    case FileExts.Whacker:
                        saber = await whackerLoader.LoadWhackerAsync(saberPath); break;
                }
                saberInstanceManager.AddSaber(saber);
            }
            Data = saber;
            return saber;
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
