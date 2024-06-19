using CustomSabersLite.Components.Managers;
using CustomSabersLite.Data;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CustomSabersLite.Utilities.AssetBundles
{
    /// <summary>
    /// Class for loading different kinds of custom saber assets
    /// </summary>
    internal class CustomSabersLoader
    {
        private readonly SaberInstanceManager saberInstanceManager;
        private readonly SaberLoader saberLoader;
        private readonly WhackerLoader whackerLoader;

        public CustomSabersLoader(SaberInstanceManager saberInstanceManager, SaberLoader saberLoader, WhackerLoader whackerLoader)
        {
            this.saberInstanceManager = saberInstanceManager;
            this.saberLoader = saberLoader;
            this.whackerLoader = whackerLoader;
        }

        public async Task<CustomSaberData> GetSaberData(string saberPath)
        {
            if (!saberInstanceManager.TryGetSaber(saberPath, out CustomSaberData saber))
            {
                saber = await LoadSaberDataAsync(saberPath);
                saberInstanceManager.AddSaber(saber);
            }
            return saber;
        }

        public async Task<IEnumerable<CustomSaberData>> LoadCustomSabersAsync(IEnumerable<string> customSaberFiles)
        {
            IList<CustomSaberData> customSabers = new List<CustomSaberData>();
            foreach (string file in customSaberFiles)
            {
                customSabers.Add(await LoadSaberDataAsync(file));
            }
            return customSabers;
        }

        private async Task<CustomSaberData> LoadSaberDataAsync(string saberPath)
        {
            CustomSaberData saberData = CustomSaberData.Default;

            switch (Path.GetExtension(saberPath))
            {
                case FileExts.Saber:
                    saberData = await saberLoader.LoadCustomSaberAsync(saberPath);
                    break;

                case FileExts.Whacker:
                    saberData = await whackerLoader.LoadWhackerAsync(saberPath);
                    break;

                default: return saberData;
            }

            if (saberData != null)
            {
                saberData.SaberPrefab.name += $" {saberData.Descriptor.SaberName}";
            }

            return saberData;
        }
    }
}
