using IPA.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CustomSabersLite.Utilities;

internal class SaberMetadataCacheMigrationManager
{
    public Task<bool> MigrationTask { get; } = Task.Run(() => Migrate(GetCurrentCacheVersion()));

    private enum CacheVersion
    {
        Current = 0,
        V1 // Cache folder used by CustomSabersLite prior to v1.13.0
    }

    private static CacheVersion GetCurrentCacheVersion()
    {
        if (Directory.Exists(Path.Combine(PluginDirs.UserData.FullName, "Cache")))
        {
            return CacheVersion.V1;
        }

        return CacheVersion.Current;
    }

    private static bool Migrate(CacheVersion cacheVersion)
    {
        if (cacheVersion == CacheVersion.Current) return true;

        try
        {
            Logger.Info("Old cache version detected, running migration for saber metadata");

            if (cacheVersion == CacheVersion.V1)
            {
                V1Migration();
            }
        }
        catch (Exception ex)
        {
            Logger.Critical("A problem occurred during saber metadata cache migration. This is not good.");
            Logger.Error(ex);
            return false;
        }

        return true;
    }

    private static void V1Migration()
    {
        var v1Dir = new DirectoryInfo(Path.Combine(PluginDirs.UserData.FullName, "Cache"));
        if (!v1Dir.Exists) return;

        v1Dir.EnumerateFiles("*.meta", SearchOption.TopDirectoryOnly).ForEach(f => f.Delete());
        if (v1Dir.EnumerateFiles("*", SearchOption.AllDirectories).Any())
            throw new Exception(
                "Version 1 cache folder contains alien files.\n" +
                $"Remove any personal files from {v1Dir.FullName[UnityGame.InstallPath.Length..]}");
        v1Dir.Delete();
    }
}
