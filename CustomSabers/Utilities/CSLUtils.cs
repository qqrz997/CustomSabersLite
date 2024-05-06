using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using System.Reflection;
using System.Threading.Tasks;

namespace CustomSabersLite.Utilities
{
    internal class CSLUtils
    {
        public static IEnumerable<string> GetFilePaths(string path, IEnumerable<string> fileExtensions, SearchOption searchOption = SearchOption.AllDirectories, bool returnShortPath = false)
        {
            IList<string> filePaths = new List<string>();

            foreach (string extension in fileExtensions)
            {
                IEnumerable<string> files = Directory.EnumerateFiles(path, "*.*", searchOption).Where(s => extension.Contains(Path.GetExtension(s).TrimEnd('.').ToLowerInvariant()));

                if (returnShortPath)
                {
                    foreach (string file in files)
                    {
                        string filePath = file.Replace(path, "");
                        if (filePath.Length > 0 && filePath.StartsWith(@"\"))
                        {
                            filePath = filePath.Substring(1, filePath.Length - 1);
                        }

                        if (!string.IsNullOrWhiteSpace(filePath) && !filePaths.Contains(filePath))
                        {
                            filePaths.Add(filePath);
                        }
                    }
                }
                else
                {
                    filePaths = filePaths.Union(files).ToList();
                }
            }

            return filePaths.Distinct();
        }

        private static Sprite nullCoverImage = null;
        public static Sprite GetNullCoverImage()
        {
            if (!nullCoverImage)
            {
                try
                {
                    nullCoverImage = ImageLoading.LoadSpriteFromResources("CustomSabersLite.Resources.null-image.png");
                    nullCoverImage.texture.wrapMode = TextureWrapMode.Clamp;
                }
                catch (Exception ex) 
                {
                    Logger.Error(ex.Message);
                }
            }
            return nullCoverImage;
        }

        private static Sprite defaultCoverImage = null;
        public static Sprite GetDefaultCoverImage()
        {
            if (!defaultCoverImage)
            {
                try
                {
                    defaultCoverImage = ImageLoading.LoadSpriteFromResources("CustomSabersLite.Resources.defaultsabers-image.png");
                    defaultCoverImage.texture.wrapMode = TextureWrapMode.Clamp;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                }
            }
            return defaultCoverImage;
        }

        public static async Task<bool> LoadCustomSaberAssembly()
        {
            try
            {
                byte[] customSaberAssembly = await ResourceLoading.LoadFromResourceAsync("CustomSabersLite.Resources.CustomSaber.dll");

                Assembly.Load(customSaberAssembly);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Critical($"Couldn't load CustomSaber.dll\n{ex}");
                return false;
            }
        }
    }
}
