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

        public async Task<CustomSaberData> LoadWhackerAsync(string relativePath)
        {
            string path = Path.Combine(sabersPath, relativePath);
            
            if (!File.Exists(path))
            {
                return CustomSaberData.ForDefaultSabers();
            }

            ZipArchive archive = ZipFile.OpenRead(path);
            ZipArchiveEntry json = archive.Entries.Where(x => x.FullName.EndsWith(".json")).FirstOrDefault();

            StreamReader jsonStream = new StreamReader(json.Open());
            WhackerObject whacker = (WhackerObject)new JsonSerializer().Deserialize(jsonStream, typeof(WhackerObject));
            jsonStream.Dispose();

            ZipArchiveEntry bundleEntry = archive.GetEntry(whacker.pcFileName);
            ZipArchiveEntry thumbEntry = archive.GetEntry(whacker.descriptor.coverImage);

            Stream bundleStream = bundleEntry.Open();
            AssetBundle bundle = await bundleLoader.LoadBundleAsync(bundleStream);
            bundleStream.Dispose();

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

            await ShaderRepairUtils.RepairSaberShadersAsync(saberPrefab);

            return new CustomSaberData(relativePath, saberPrefab, descriptor, CustomSaberType.Whacker);
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
