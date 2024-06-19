using CustomSabersLite.Components.Game;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities.AssetBundles;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.Components.Managers
{
    internal class LiteSaberSet
    {
        private readonly SaberInstanceManager saberInstanceManager;
        private readonly CustomSabersLoader customSabersLoader;

        public LiteSaberSet(SaberInstanceManager saberInstanceManager, CustomSabersLoader customSabersLoader)
        {
            this.saberInstanceManager = saberInstanceManager;
            this.customSabersLoader = customSabersLoader;
        }

        private GameObject leftSaberPrefab = null;
        private GameObject rightSaberPrefab = null;

        public CustomSaberData Data { get; private set; }

        public LiteSaber NewSaberForSaberType(SaberType saberType)
        {
            GameObject original = saberType == SaberType.SaberA ? leftSaberPrefab : rightSaberPrefab;
            return original ? GameObject.Instantiate(original).AddComponent<LiteSaber>() : null;
        }

        public async Task SetSabers(string saberPath)
        {
            CustomSaberData saberData = await GetSaberData(saberPath);
            if (saberData.SaberPrefab != null)
            {
                leftSaberPrefab = saberData.SaberPrefab.transform.Find("LeftSaber")?.gameObject;
                rightSaberPrefab = saberData.SaberPrefab.transform.Find("RightSaber")?.gameObject;
            }
        }

        public async Task<CustomSaberData> GetSaberData(string saberPath) =>
            Data = await customSabersLoader.GetSaberData(saberPath);
    }
}
