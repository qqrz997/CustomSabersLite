using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomSabersLite.Utilities
{
    internal static class CustomSaberUtils
    {
        public static async Task<bool> LoadCustomSaberAssembly()
        {
            try
            {
                Assembly.Load(await ResourceLoading.LoadFromResourceAsync("CustomSabersLite.Resources.CustomSaber.dll"));
                return true;
            }
            catch (Exception ex)
            {
                Logger.Critical($"Couldn't load CustomSaber.dll\n{ex}");
                return false;
            }
        }

        public static SaberDescriptor New(this SaberDescriptor descriptor, string saberName, string authorName, string description = null, Sprite coverImage = null)
        {
            descriptor = new SaberDescriptor
            {
                SaberName = saberName,
                AuthorName = authorName,
                Description = description,
                CoverImage = coverImage
            };
            return descriptor;
        }
    }
}
