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

    public bool ShowFavourites { get; set; }

    private ISaberListCell[] TrailListDefaultChoices { get; } =
    [
        new SaberListInfoCell("Custom", "Use the trail of the selected saber", TrailDurationIcon, "custom"),
        new SaberListInfoCell("Default", "Beat Games", DefaultCoverImage, "default"),
        new SaberListInfoCell("None", "Use no trail", BlankSprite, null)
    ];

    private ISaberListCell[] SaberListDefaultChoices { get; } =
    [
        new SaberListInfoCell("Default", "Beat Games", DefaultCoverImage, null)
    ];

    public void Refresh()
    {
        list.Clear();
        saberListFolderManager.Refresh();
    }

    public IEnumerable<ISaberListCell> UpdateUnsortedList() => PopulateUnsortedList();

    public IEnumerable<ISaberListCell> UpdateList(SaberListFilterOptions filterOptions) =>
        PopulateList(filterOptions, filterOptions.Trails switch
        {
            false => SaberListDefaultChoices,
            true => TrailListDefaultChoices
        });

    public void OpenFolder(SaberListDirectoryCell directoryCell)
    {
        saberListFolderManager.CurrentDirectory = directoryCell.DirectoryInfo;
        ShowFavourites = false;
    }

    public void OpenFolder(SaberListUpDirectoryCell upDirectoryCell)
    {
        saberListFolderManager.CurrentDirectory = upDirectoryCell.DirectoryInfo;
        ShowFavourites = false;
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
        return Math.Max(0, index);
    }

    public ISaberListCell? SelectFromUnsortedData(int row) => unsortedList.ElementAtOrDefault(row);
    public ISaberListCell? SelectFromCurrentList(int row) => list.ElementAtOrDefault(row);

    public bool CurrentListContains(string? saberHash) =>
        !string.IsNullOrEmpty(saberHash) && list.Any(i => i.Value == saberHash);

    private IEnumerable<ISaberListCell> PopulateList(
        SaberListFilterOptions filterOptions,
        IEnumerable<ISaberListCell> defaultChoices)
    {
        list.Clear();

        if (ShowFavourites)
        {
            list.Add(new SaberListUpDirectoryCell(directoryManager.CustomSabers));
            list.AddRange(saberMetadataCache
                .GetSortedData(filterOptions with { Favourites = true})
                .Select(meta => new SaberListInfoCell(meta)));
            return list;
        }
        
        var inTopDirectory = saberListFolderManager.InTopDirectory;

        list.Add(inTopDirectory 
            ? new SaberListFavouritesCell() 
            : new SaberListUpDirectoryCell(saberListFolderManager.ParentDirectory));

        list.AddRange(saberListFolderManager
            .CurrentDirectorySubDirectories
            .Select(dir => new SaberListDirectoryCell(dir)));

        if (inTopDirectory)
        {
            list.AddRange(defaultChoices);
        }
        
        list.AddRange(saberMetadataCache
            .GetSortedData(filterOptions)
            .Where(m => m.SaberFile.FileInfo.DirectoryName == saberListFolderManager.CurrentDirectory.FullName)
            .Select(m => new SaberListInfoCell(m)));
        
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
