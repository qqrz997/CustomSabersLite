using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomSabersLite.Menu.Views;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
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

    private readonly List<ISaberListCell> unsortedList = [];
    private readonly List<ISaberListCell> list = [];
    private readonly Dictionary<SaberValue, int> cellValueIndexesMap = [];

    private ISaberListCell[] SaberListDefaultChoices { get; } =
    [
        new SaberListInfoCell("Default", "Beat Games", DefaultCoverImage, new DefaultSaberValue())
    ];

    private ISaberListCell[] TrailListDefaultChoices { get; } =
    [
        new SaberListInfoCell("Custom", "Use the trail of the selected saber", TrailDurationIcon, new CustomTrailValue()),
        new SaberListInfoCell("Default", "Beat Games", DefaultCoverImage, new DefaultSaberValue()),
        new SaberListInfoCell("None", "Use no trail", BlankSprite, new NoTrailValue())
    ];
    
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

    public bool ShowFavourites { get; set; }

    public void Refresh()
    {
        list.Clear();
        saberListFolderManager.Refresh();
    }

    public IEnumerable<ISaberListCell> UpdateUnsortedList() => PopulateUnsortedList();

    public IEnumerable<ISaberListCell> UpdateList(SaberListFilterOptions filterOptions)
    {
        list.Clear();
        cellValueIndexesMap.Clear();

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

        list.ForEach((cell, i) =>
        {
            if (cell.TryGetSaberValue(out var v)) cellValueIndexesMap.Add(v, i);
        });
        
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

    public ISaberListCell? SelectFromUnsortedData(int row) => unsortedList.ElementAtOrDefault(row);
    public ISaberListCell? SelectFromCurrentList(int row) => list.ElementAtOrDefault(row);

    public int IndexForSaberValue(SaberValue saberValue) => 
        cellValueIndexesMap.GetValueOrDefault(saberValue, 0);

    public bool CurrentListContains(SaberValue value) => list.Any(cell =>
        cell.TryGetSaberValue(out var cellValue) && cellValue == value);

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
