using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using CustomSabersLite.Models;
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
        V1, // Cache folder (v0.12.7 and below)
        V2 // Cache with relative path used as key for metadata (v0.14.2 and below)
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
                CacheVersion.V2 => V2Migration,
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

    private void V2Migration()
    {
        var cacheFile = new FileInfo(Path.Combine(directoryManager.UserData.FullName, "cache"));
        if (!cacheFile.Exists) return;
        
        using var zipArchive = ZipFile.Open(cacheFile.FullName, ZipArchiveMode.Update);
        
        zipArchive.GetEntry("metadata.json")?.Delete();
    }

    private CacheVersion GetCurrentCacheVersion()
    {
        if (Directory.Exists(Path.Combine(directoryManager.UserData.FullName, "Cache"))) return CacheVersion.V1;

        var metadataVersion = GetMetadataVersionFromZip();
        if (metadataVersion <= new Version(0, 14, 3))
            return CacheVersion.V2;
        
        return CacheVersion.Current;
    }

    private Version GetMetadataVersionFromZip()
    {
        var cacheFile = new FileInfo(Path.Combine(directoryManager.UserData.FullName, "cache"));
        if (!cacheFile.Exists) return new(0, 0, 0);
        
        using var zipArchive = ZipFile.OpenRead(cacheFile.FullName);
        using var metadataStream = zipArchive.GetEntry("metadata.json")?.Open();
        if (metadataStream == null) return new(0, 0, 0);

        var versionString = JsonReading.DeserializeStream<CacheFileModel>(metadataStream)?.Version;
        
        return versionString is null ? new(0, 0, 0) : new Version(versionString);
    }
}
