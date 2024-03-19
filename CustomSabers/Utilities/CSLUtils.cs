using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using CustomSabersLite.Configuration;
using IPA.Utilities;
using System.Reflection;
using System.Threading.Tasks;
using Zenject;

namespace CustomSabersLite.Utilities
{
    internal class CSLUtils
    {
        public static IEnumerable<string> GetFileNames(string path, IEnumerable<string> filters, SearchOption searchOption, bool returnShortPath = false)
        {
            IList<string> filePaths = new List<string>();

            foreach (string filter in filters)
            {
                IEnumerable<string> files = Directory.GetFiles(path, filter, searchOption);

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
                    Logger.Error(ex.ToString());
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
                    Logger.Error(ex.ToString());
                }
            }
            return defaultCoverImage;
        }

        public static bool CheckMultiplayer()
        {
            // todo - multiplayer support
            if (GameObject.Find("MultiplayerController"))
            {
                Logger.Warn("Multiplayer is currently not supported for custom sabers.");
                return true;
            }
            return false;
        }

        public static async Task<bool> LoadCustomSaberAssembly()
        {
            try
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CustomSabersLite.Resources.CustomSaber.dll");
                
                if (stream == null)
                {
                    return false;
                }

                byte[] result = new byte[stream.Length];
                await stream.ReadAsync(result, 0, result.Length);

                Assembly.Load(result);

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
