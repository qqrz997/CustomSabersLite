using CustomSabersLite.Components.Game;
using CustomSabersLite.Data;
using CustomSabersLite.Utilities.AssetBundles;
using CustomSabersLite.Utilities.Extensions;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.Components.Managers
{
    internal class LiteSaberSet
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

        public CustomSaberType Type { get; private set; }

        public LiteSaber CustomSaberForSaberType(SaberType saberType) =>
            saberType == SaberType.SaberA ? LeftSaber : RightSaber;

        public async Task SetSabersFromPath(string saberPath) => 
            SetSabersFromObject(await GetSaberObject(saberPath));

        public async Task InstantiateSabers(string saberPath) => 
            SetSabersFromObject(GameObject.Instantiate(await GetSaberObject(saberPath)));

        public async Task<GameObject> GetSaberObject(string saberPath)
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
            Type = saber.Type;
            return saber.SabersObject;
        }

        private void SetSabersFromObject(GameObject sabersObject)
        {
            LeftSaber = FromSabersObject(sabersObject, "LeftSaber");
            RightSaber = FromSabersObject(sabersObject, "RightSaber");
        }

        private LiteSaber FromSabersObject(GameObject sabersObject, string saberName) =>
            sabersObject.transform.Find(saberName)?.gameObject.TryGetComponentOrDefault<LiteSaber>();
    }
}
