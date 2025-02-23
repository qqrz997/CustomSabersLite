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
        new SaberListInfoCell("Custom", "Use the trail of the selected saber", TrailDurationIcon, new CustomTrailValue()),
        new SaberListInfoCell("Default", "Beat Games", DefaultCoverImage, new DefaultSaberValue()),
        new SaberListInfoCell("None", "Use no trail", BlankSprite, new NoTrailValue())
    ];

    private ISaberListCell[] SaberListDefaultChoices { get; } =
    [
        new SaberListInfoCell("Default", "Beat Games", DefaultCoverImage, new DefaultSaberValue())
    ];

    public void Refresh()
    {
        list.Clear();
        saberListFolderManager.Refresh();
    }

    public IEnumerable<ISaberListCell> UpdateUnsortedList() => PopulateUnsortedList();

    public IEnumerable<ISaberListCell> UpdateList(SaberListFilterOptions filterOptions)
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
            list.AddRange(filterOptions.Trails ? TrailListDefaultChoices : SaberListDefaultChoices);
        }
        
        list.AddRange(saberMetadataCache
            .GetSortedData(filterOptions)
            .Where(m => m.SaberFile.FileInfo.DirectoryName == saberListFolderManager.CurrentDirectory.FullName)
            .Select(m => new SaberListInfoCell(m)));
        
        return list;
    }

    public void OpenFolder(DirectoryInfo directoryInfo)
    {
        saberListFolderManager.CurrentDirectory = directoryInfo;
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

    public int IndexForSaberHash(string? saberHash) =>
        saberHash is null ? 0
        : Math.Max(0, list.IndexOf(list.FirstOrDefault(cell => FilterForSaberValue(cell, saberHash))));

    public ISaberListCell? SelectFromUnsortedData(int row) => unsortedList.ElementAtOrDefault(row);
    public ISaberListCell? SelectFromCurrentList(int row) => list.ElementAtOrDefault(row);

    public bool CurrentListContains(string? saberHash) =>
        saberHash != null && list.Any(cell => FilterForSaberValue(cell, saberHash));
    
    private static bool FilterForSaberValue(ISaberListCell cell, string value) =>
        cell is SaberListInfoCell { Value: SaberHash saberHash } && saberHash.Hash == value;

    private IEnumerable<ISaberListCell> PopulateUnsortedList()
    {
        unsortedList.Clear();
        unsortedList.AddRange(SaberListDefaultChoices);
        unsortedList.AddRange(saberMetadataCache
            .GetSortedData(SaberListFilterOptions.Default)
            .Select(m => new SaberListInfoCell(m)));
        return unsortedList;
    }
}
