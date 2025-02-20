using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using IPA.Utilities;

namespace CustomSabersLite.Utilities.Services;

internal class SaberMetadataCacheMigrationManager
{
    private readonly DirectoryManager directoryManager;

    public SaberMetadataCacheMigrationManager(DirectoryManager directoryManager)
    {
        this.directoryManager = directoryManager;

        var cacheVersion = GetCurrentCacheVersion();
        MigrationTask = Task.Run(() => Migrate(cacheVersion));
    }
    
    public Task<bool> MigrationTask { get; }

    private enum CacheVersion
    {
        Current = 0,
        V1 // Cache folder used by CustomSabersLite prior to v0.13.0
    }

    private bool Migrate(CacheVersion cacheVersion)
    {
        if (cacheVersion == CacheVersion.Current) return true;
        Logger.Notice("Old cache version detected, running migration for saber metadata");

        try
        {
            Action? migration = cacheVersion switch
            {
                CacheVersion.V1 => V1Migration,
                _ => null,
            };
            migration?.Invoke();
            return true;
        }
        catch (Exception ex)
        {
            Logger.Critical("A problem occurred during saber metadata cache migration. This is not good.");
            Logger.Error(ex);
            return false;
        }
    }

    private void V1Migration()
    {
        var v1Dir = new DirectoryInfo(Path.Combine(directoryManager.UserData.FullName, "Cache"));
        if (!v1Dir.Exists) return;

        v1Dir.EnumerateFiles("*.meta", SearchOption.TopDirectoryOnly).ForEach(f => f.Delete());
        if (v1Dir.EnumerateFiles("*", SearchOption.AllDirectories).Any())
            throw new(
                "Version 1 cache folder contains alien files.\n" +
                $"Remove any personal files from {v1Dir.FullName[UnityGame.InstallPath.Length..]}");
        v1Dir.Delete();
    }

    private CacheVersion GetCurrentCacheVersion() =>
        Directory.Exists(Path.Combine(directoryManager.UserData.FullName, "Cache")) ? CacheVersion.V1
        : CacheVersion.Current;
}
