using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomSabersLite.Models;
using CustomSabersLite.Services;
using CustomSabersLite.Utilities.Common;
using Zenject;

namespace CustomSabersLite.Menu.Views;

internal class SaberFoldersManager : IInitializable, IDisposable
{
    private readonly DirectoryManager directoryManager;
    private readonly SaberMetadataCache saberMetadataCache;
    private readonly MetadataCacheLoader metadataCacheLoader;
    private readonly List<DirectoryInfo> customSabersSubDirs = [];
    
    public SaberFoldersManager(
        DirectoryManager directoryManager,
        SaberMetadataCache saberMetadataCache,
        MetadataCacheLoader metadataCacheLoader)
    {
        this.directoryManager = directoryManager;
        this.saberMetadataCache = saberMetadataCache;
        this.metadataCacheLoader = metadataCacheLoader;
        CurrentDirectory = directoryManager.CustomSabers;
    }

    public DirectoryInfo CurrentDirectory { get; set; }
    public DirectoryInfo ParentDirectory => CurrentDirectory.Parent ?? directoryManager.CustomSabers;
    public bool InTopDirectory => CurrentDirectory.FullName == directoryManager.CustomSabers.FullName;

    public IEnumerable<DirectoryInfo> CurrentDirectorySubDirectories => 
        customSabersSubDirs.Where(dir => dir.Parent?.FullName == CurrentDirectory.FullName);

    public void Initialize()
    {
        metadataCacheLoader.LoadingProgressChanged += LoadingProgressChanged;
        Refresh();
    }

    public void Dispose()
    {
        metadataCacheLoader.LoadingProgressChanged -= LoadingProgressChanged;
    }

    public void Refresh()
    {
        var saberDirs = saberMetadataCache
            .GetSortedData(SaberListFilterOptions.Default)
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

    private void LoadingProgressChanged(MetadataCacheLoader.Progress progress)
    {
        if (progress.Completed) Refresh();
    }
}