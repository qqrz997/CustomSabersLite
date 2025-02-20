using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomSabersLite.Menu.Views;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Services;
using static BeatSaberMarkupLanguage.Utilities.ImageResources;
using static CustomSabersLite.Utilities.Common.CSLResources;

namespace CustomSabersLite.Menu;

internal class SaberListManager
{
    private readonly SaberPrefabCache saberPrefabCache;
    private readonly SaberMetadataCache saberMetadataCache;
    private readonly DirectoryManager directoryManager;
    private readonly SaberListFolderManager saberListFolderManager;
    
    public SaberListManager(
        SaberPrefabCache saberPrefabCache,
        SaberMetadataCache saberMetadataCache,
        DirectoryManager directoryManager,
        SaberListFolderManager saberListFolderManager)
    {
        this.saberPrefabCache = saberPrefabCache;
        this.saberMetadataCache = saberMetadataCache;
        this.directoryManager = directoryManager;
        this.saberListFolderManager = saberListFolderManager;
    }

    private readonly List<ISaberListCell> unsortedList = [];
    private readonly List<ISaberListCell> list = [];

    public void Clear() => list.Clear();

    public IEnumerable<ISaberListCell> UpdateUnsortedList() => PopulateUnsortedList();

    public IEnumerable<ISaberListCell> UpdateList(SaberListFilterOptions filterOptions) =>
        filterOptions.SaberListType switch
        {
            SaberListType.Sabers => PopulateSaberList(filterOptions),
            SaberListType.Trails => PopulateTrailList(filterOptions),
            _ => throw new ArgumentOutOfRangeException(nameof(filterOptions.SaberListType))
        };

    public void OpenDirectory(SaberListDirectoryCell directoryCell)
    {
        saberListFolderManager.CurrentDirectory = directoryCell.DirectoryInfo;
    }
    
    public void DeleteSaber(string? saberHash)
    {
        var saberFile = saberMetadataCache.GetOrDefault(saberHash)?.SaberFile;
        if (saberFile is null || !saberFile.FileInfo.Exists)
        {
            return;
        }

        var destinationFile = new FileInfo(Path.Combine(directoryManager.DeletedSabers.FullName, saberFile.FileInfo.Name));
        if (destinationFile.Exists)
        {
            destinationFile.Delete();
        }

        saberFile.FileInfo.MoveTo(destinationFile.FullName);

        saberMetadataCache.Remove(saberFile.Hash);
        saberPrefabCache.UnloadSaber(saberFile.Hash);
    }

    public int IndexForSaberHash(string? saberHash)
    {
        if (string.IsNullOrWhiteSpace(saberHash)) return 0;
        int index = list.IndexOf(list.FirstOrDefault(i => i.Value == saberHash));
        return index != -1 ? index : 0;
    }

    public ISaberListCell? SelectFromUnsortedData(int row) => unsortedList.ElementAtOrDefault(row);
    public ISaberListCell? SelectFromCurrentList(int row) => list.ElementAtOrDefault(row);

    public bool CurrentListContains(string? saberHash) =>
        !string.IsNullOrEmpty(saberHash) && list.Any(i => i.Value == saberHash);

    private IEnumerable<ISaberListCell> PopulateSaberList(SaberListFilterOptions filterOptions)
    {
        list.Clear();
        list.Add(new SaberListInfoCell("Default", "Beat Games", DefaultCoverImage, null));
        list.AddRange(saberListFolderManager.CurrentDirectorySubDirectories.Select(dir => new SaberListDirectoryCell(dir)));
        var data = saberMetadataCache.GetSortedData(filterOptions).ToList();
        Logger.Warn($"Displaying data sorted");
        foreach (var x in data)
        {
            Logger.Info($"{x.SaberFile.FileInfo.FullName ?? "null"}");
        }

        Logger.Warn($"Displaying data, current directory: {saberListFolderManager.CurrentDirectory.FullName}");
        var inDirectory = data.Where(m => m.SaberFile.FileInfo.DirectoryName == saberListFolderManager.CurrentDirectory.FullName).ToList();
        foreach (var x in inDirectory)
        {
            Logger.Info($"{x.Descriptor.SaberName.FullName}: {x.SaberFile.FileInfo.DirectoryName ?? "null"}");
        }
        
        list.AddRange(inDirectory.Select(m => new SaberListInfoCell(m)));
        // list.AddRange(
        //     saberMetadataCache
        //         .GetSortedData(filterOptions)
        //         .Where(m => m.SaberFile.FileInfo.Directory?.FullName == saberListFolderManager.CurrentDirectory.FullName)
        //         .Select(m => new SaberListInfoCell(m)));
        return list;
    }

    private IEnumerable<ISaberListCell> PopulateTrailList(SaberListFilterOptions filterOptions)
    {
        list.Clear();
        list.Add(new SaberListInfoCell("Custom", "Use the trail of the selected saber", TrailDurationIcon, "custom"));
        list.Add(new SaberListInfoCell("Default", "Beat Games", DefaultCoverImage, "default"));
        list.Add(new SaberListInfoCell("None", "Use no trail", BlankSprite, null));
        list.AddRange(saberMetadataCache.GetSortedData(filterOptions).Select(m => new SaberListInfoCell(m)));
        return list;
    }

    private IEnumerable<ISaberListCell> PopulateUnsortedList()
    {
        unsortedList.Clear();
        unsortedList.Add(new SaberListInfoCell("Default", "Beat Games", DefaultCoverImage, null));
        unsortedList.AddRange(
            saberMetadataCache.GetSortedData(SaberListFilterOptions.Default).Select(m => new SaberListInfoCell(m)));
        return unsortedList;
    }
}
