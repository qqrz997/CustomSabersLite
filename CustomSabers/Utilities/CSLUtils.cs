using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using CustomSabersLite.Configuration;
using IPA.Utilities;
using System.Reflection;
using System.Threading.Tasks;

namespace CustomSabersLite.Utilities
{
    public static class CSLUtils
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
                    Plugin.Log.Error(ex);
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
                    Plugin.Log.Error(ex);
                }
            }
            return defaultCoverImage;
        }

        public static void HideTrail(SaberTrail trail)
        {
            trail.enabled = false;
        }

        public static void SetTrailDuration(SaberTrail trail, float trailDuration = 0.4f)
        {
            if (CSLConfig.Instance.OverrideTrailDuration)
            {
                trailDuration = CSLConfig.Instance.TrailDuration / 100f * trailDuration;
            }

            if (trailDuration == 0)
            {
                HideTrail(trail);
            }
            else
            {
                ReflectionUtil.SetField(trail, "_trailDuration", trailDuration);
            }
        }

        public static void SetWhiteTrailDuration(SaberTrail defaultTrail, float whiteSectionMaxDuration = 0.03f)
        {
            if (CSLConfig.Instance.DisableWhiteTrail)
            {
                // setting the trail duration to 0 doesn't completely hide trails, i assume this works the same but it's small enough to be completely unnoticeable
                whiteSectionMaxDuration = 0f; // Could add config to adjust the white section length for fun
            }
            ReflectionUtil.SetField(defaultTrail, "_whiteSectionMaxDuration", whiteSectionMaxDuration);
        }

        public static bool CheckMultiplayer()
        {
            // todo - multiplayer support
            if (GameObject.Find("MultiplayerController"))
            {
                Plugin.Log.Warn("Multiplayer is currently not supported for custom sabers.");
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
            catch (Exception e)
            {
                Plugin.Log.Error(e);
                return false;
            }
        }
    }
}
