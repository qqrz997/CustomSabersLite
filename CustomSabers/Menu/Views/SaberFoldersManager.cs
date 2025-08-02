using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomSabersLite.Services;
using CustomSabersLite.Utilities.Common;
using SabersLib.Models;
using SabersLib.Services;
using Zenject;

namespace CustomSabersLite.Menu.Views;

internal class SaberFoldersManager : IInitializable, IDisposable
{
    private readonly DirectoryManager directoryManager;
    private readonly ISaberMetadataCache saberMetadataCache;
    private readonly ISaberMetadataLoader saberMetadataLoader;
    private readonly List<DirectoryInfo> customSabersSubDirs = [];
    
    public SaberFoldersManager(
        DirectoryManager directoryManager,
        ISaberMetadataCache saberMetadataCache,
        ISaberMetadataLoader saberMetadataLoader)
    {
        this.directoryManager = directoryManager;
        this.saberMetadataCache = saberMetadataCache;
        this.saberMetadataLoader = saberMetadataLoader;
        CurrentDirectory = directoryManager.CustomSabers;
    }

    public DirectoryInfo CurrentDirectory { get; set; }
    public DirectoryInfo ParentDirectory => CurrentDirectory.Parent ?? directoryManager.CustomSabers;
    public bool InTopDirectory => CurrentDirectory.FullName == directoryManager.CustomSabers.FullName;

    public IEnumerable<DirectoryInfo> CurrentDirectorySubDirectories => 
        customSabersSubDirs.Where(dir => dir.Parent?.FullName == CurrentDirectory.FullName);

    public void Initialize()
    {
        saberMetadataLoader.LoadingProgressChanged += LoadingProgressChanged;
        Refresh();
    }

    public void Dispose()
    {
        saberMetadataLoader.LoadingProgressChanged -= LoadingProgressChanged;
    }

    public void Refresh()
    {
        var saberDirs = saberMetadataCache
            .GetRefreshedMetadata()
            .SelectMany(GetParentDirectories)
            .DistinctByPath();
        
        customSabersSubDirs.Clear();
        customSabersSubDirs.AddRange(saberDirs);
    }

    private  IEnumerable<DirectoryInfo> GetParentDirectories(CustomSaberMetadata saberMetadata)
    {
        var dir = saberMetadata.SaberFile.FileInfo.Directory;

        while (dir != null && dir.FullName != directoryManager.CustomSabers.FullName)
        {
            yield return dir;
            dir = dir.Parent;
        }
    }

    private void LoadingProgressChanged(MetadataLoaderProgress progress)
    {
        if (progress.Completed) Refresh();
    }
}