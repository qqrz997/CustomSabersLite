using System;
using System.Reflection;
using System.Threading.Tasks;
using CustomSabersLite.Utilities.Services;

namespace CustomSabersLite.Utilities.Common;

internal static class CustomSaberUtils
{
    public static async Task<bool> LoadCustomSaberAssembly()
    {
        try
        {
            Assembly.Load(await ResourceLoading.GetResourceAsync("CustomSabersLite.Resources.CustomSaber.dll"));
            return true;
        }
        catch (Exception ex)
        {
            Logger.Critical($"Couldn't load CustomSaber.dll\n{ex}");
            return false;
        }
    }
}
