using System.IO;
using System.IO.Compression;
using System.Linq;
using Newtonsoft.Json;
using CustomSabersLite.Data;
using UnityEngine;
using AssetBundleLoadingTools.Utilities;
using System.Threading.Tasks;

namespace CustomSabersLite.Utilities.AssetBundles
{
    /// <summary>
    /// Class for loading .whacker files
    /// </summary>
    internal class WhackerLoader
    {
        private readonly BundleLoader bundleLoader;
        private readonly string sabersPath;

        public WhackerLoader(PluginDirs pluginDirs, BundleLoader bundleLoader)
        {
            this.bundleLoader = bundleLoader;
            sabersPath = pluginDirs.CustomSabers.FullName;
        }

        /// <summary>
        /// Loads a custom saber from a .whacker file
        /// </summary>
        /// <param name="relativePath">Path to the .whacker file in the CustomSabers folder</param>
        /// <returns><seealso cref="CustomSaberData.ForDefaultSabers"/> if a custom saber failed to load</returns>
        public async Task<CustomSaberData> LoadWhackerAsync(string relativePath)
        {
            string path = Path.Combine(sabersPath, relativePath);
            if (!File.Exists(path))
            {
                return CustomSaberData.ForDefaultSabers();
            }

            ZipArchive archive = ZipFile.OpenRead(path);
            ZipArchiveEntry json = archive.Entries.Where(x => x.FullName.EndsWith(".json")).FirstOrDefault();

            Stream jsonStream = json.Open();
            StreamReader jsonStreamReader = new StreamReader(jsonStream);
            WhackerObject whacker = (WhackerObject)new JsonSerializer().Deserialize(jsonStreamReader, typeof(WhackerObject));

            ZipArchiveEntry bundleEntry = archive.GetEntry(whacker.pcFileName);
            ZipArchiveEntry thumbEntry = archive.GetEntry(whacker.descriptor.coverImage);

            Stream bundleStream = bundleEntry.Open();
            AssetBundle bundle = await bundleLoader.LoadBundleAsync(bundleStream);
            if (bundle is null)
            {
                return CustomSaberData.ForDefaultSabers();
            }
            GameObject saberPrefab = await AssetBundleExtensions.LoadAssetAsync<GameObject>(bundle, "_Whacker");
            if (saberPrefab is null)
            {
                bundle.Unload(true);
                return CustomSaberData.ForDefaultSabers();
            }
            saberPrefab.hideFlags = HideFlags.DontUnloadUnusedAsset;

            SaberDescriptor descriptor = new SaberDescriptor
            {
                SaberName = whacker.descriptor.objectName,
                AuthorName = whacker.descriptor.author,
                Description = whacker.descriptor.description,
                CoverImage = thumbEntry is null ? null : await GetCoverFromArchive(thumbEntry)
            };
            bundle.Unload(false);

            bool missingShaders = await ShaderRepairUtils.RepairSaberShadersAsync(saberPrefab);

            archive.Dispose();
            jsonStream.Dispose();
            jsonStreamReader.Dispose();
            bundleStream.Dispose();
            
            return new CustomSaberData(relativePath, saberPrefab, descriptor, CustomSaberType.Whacker) { MissingShaders = missingShaders };
        }

        private async Task<Sprite> GetCoverFromArchive(ZipArchiveEntry thumbEntry)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await thumbEntry.Open().CopyToAsync(memoryStream);
                return memoryStream.ToArray().LoadImage();
            }
        }
    }
}
