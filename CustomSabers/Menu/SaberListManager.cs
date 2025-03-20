using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomSabersLite.Menu.Views;
using CustomSabersLite.Models;
using CustomSabersLite.Services;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Extensions;
using static CustomSabersLite.Utilities.Common.CSLResources;

namespace CustomSabersLite.Menu;

internal class SaberListManager
{
    private readonly SaberPrefabCache saberPrefabCache;
    private readonly SaberMetadataCache saberMetadataCache;
    private readonly DirectoryManager directoryManager;
    private readonly SaberFoldersManager saberFoldersManager;

    private readonly List<IListCellInfo> sortedList = [];
    private readonly Dictionary<SaberValue, int> sortedListIndexMap = [];
    private readonly List<IListCellInfo> unsortedList = [];
    private readonly Dictionary<SaberValue, int> unsortedListIndexMap = [];

    private IListCellInfo[] SaberListDefaultChoices { get; } =
    [
        new ListInfoCellInfo("Default", "Beat Games", DefaultCoverImage, new DefaultSaberValue())
    ];

    private IListCellInfo[] TrailListDefaultChoices { get; } =
    [
        new ListInfoCellInfo("Custom", "Use the trail of the selected saber", CustomTrailIcon, new CustomTrailValue()),
        new ListInfoCellInfo("Default", "Beat Games", DefaultCoverImage, new DefaultSaberValue()),
        new ListInfoCellInfo("None", "Use no trail", NoTrailIcon, new NoTrailValue())
    ];
    
    public SaberListManager(
        SaberPrefabCache saberPrefabCache,
        SaberMetadataCache saberMetadataCache,
        DirectoryManager directoryManager,
        SaberFoldersManager saberFoldersManager)
    {
        this.saberPrefabCache = saberPrefabCache;
        this.saberMetadataCache = saberMetadataCache;
        this.directoryManager = directoryManager;
        this.saberFoldersManager = saberFoldersManager;
    }

    public bool ShowFavourites { get; set; }

    public void Refresh()
    {
        sortedList.Clear();
        OpenFolder(directoryManager.CustomSabers);
    }

    public IEnumerable<IListCellInfo> UpdateList(SaberListFilterOptions filterOptions) => 
        PopulateList(sortedList, sortedListIndexMap, filterOptions);
    public IEnumerable<IListCellInfo> UpdateUnsortedList() =>
        PopulateList(unsortedList, unsortedListIndexMap, SaberListFilterOptions.Default);

    public void OpenFolder(DirectoryInfo directoryInfo)
    {
        saberFoldersManager.CurrentDirectory = directoryInfo;
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

    public IListCellInfo? SelectFromCurrentList(int row) => 
        sortedList.ElementAtOrDefault(row);
    public IListCellInfo? SelectFromUnsortedList(int row) => 
        unsortedList.ElementAtOrDefault(row);

    public int IndexForSaberValue(SaberValue saberValue) => 
        sortedListIndexMap.GetValueOrDefault(saberValue, 0);
    public int IndexForSaberValueUnsorted(SaberValue saberValue) => 
        unsortedListIndexMap.GetValueOrDefault(saberValue, 0);

    public bool CurrentListContains(SaberValue value) => 
        sortedList.Any(cell => cell.TryGetSaberValue(out var cellValue) && cellValue == value);
    public bool UnsortedListContains(SaberValue value) =>
        unsortedList.Any(cell => cell.TryGetSaberValue(out var cellValue) && cellValue == value);
    
    private IEnumerable<IListCellInfo> PopulateList(
        List<IListCellInfo> list,
        Dictionary<SaberValue, int> valueIndexMap, 
        SaberListFilterOptions filterOptions)
    {
        list.Clear();
        valueIndexMap.Clear();

        if (ShowFavourites)
        {
            list.Add(new ListUpDirectoryCellInfo(directoryManager.CustomSabers));
            list.AddRange(saberMetadataCache
                .GetSortedData(filterOptions with { Favourites = true})
                .Select(meta => new ListInfoCellInfo(meta)));
            return list;
        }
        
        var inTopDirectory = saberFoldersManager.InTopDirectory;

        list.Add(inTopDirectory 
            ? new ListFavouritesCellInfo() 
            : new ListUpDirectoryCellInfo(saberFoldersManager.ParentDirectory));

        list.AddRange(saberFoldersManager
            .CurrentDirectorySubDirectories
            .Select(dir => new ListDirectoryCellInfo(dir)));

        if (inTopDirectory)
        {
            list.AddRange(filterOptions.Trails ? TrailListDefaultChoices : SaberListDefaultChoices);
        }
        
        list.AddRange(saberMetadataCache
            .GetSortedData(filterOptions)
            .Where(m => m.SaberFile.FileInfo.DirectoryName == saberFoldersManager.CurrentDirectory.FullName)
            .Select(m => new ListInfoCellInfo(m)));

        list.ForEach((cell, i) =>
        {
            if (cell.TryGetSaberValue(out var v)) valueIndexMap.Add(v, i);
        });
        
        return list;
    }
}
