using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using CustomSabersLite.Menu.Views;
using CustomSabersLite.Models;
using CustomSabersLite.Services;
using CustomSabersLite.Utilities.Extensions;
using SabersLib.Models;
using SabersLib.Services;
using static CustomSabersLite.Utilities.Common.PluginResources;

namespace CustomSabersLite.Menu;

internal class SaberListManager
{
    private readonly IPrefabCache prefabCache;
    private readonly ISaberMetadataCache saberMetadataCache;
    private readonly DirectoryManager directoryManager;
    private readonly SaberFoldersManager saberFoldersManager;
    private readonly FavouritesManager favouritesManager;

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
        new ListInfoCellInfo("None", "Use no trail", NoTrailIcon, new NoTrailValue()),
        new ListInfoCellInfo("Default", "Beat Games", DefaultCoverImage, new DefaultSaberValue())
    ];
    
    public SaberListManager(
        IPrefabCache prefabCache,
        ISaberMetadataCache saberMetadataCache,
        DirectoryManager directoryManager,
        SaberFoldersManager saberFoldersManager, 
        FavouritesManager favouritesManager)
    {
        this.prefabCache = prefabCache;
        this.saberMetadataCache = saberMetadataCache;
        this.directoryManager = directoryManager;
        this.saberFoldersManager = saberFoldersManager;
        this.favouritesManager = favouritesManager;
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
        prefabCache.UnloadPrefab(saberFile.Hash);
    }

    public bool TrySelectSorted(int row, [NotNullWhen(true)] out IListCellInfo? cell) =>
        TrySelect(sortedList, row, out cell);
    public bool TrySelectUnsorted(int row, [NotNullWhen(true)] out IListCellInfo? cell) => 
        TrySelect(unsortedList, row, out cell);
    
    public int IndexForSaberValue(SaberValue saberValue) => 
        sortedListIndexMap.TryGetValue(saberValue, out var idx) ? idx
        : sortedListIndexMap.GetValueOrDefault(new DefaultSaberValue(), 0);
    public int IndexForSaberValueUnsorted(SaberValue saberValue) => 
        unsortedListIndexMap.TryGetValue(saberValue, out int idx) ? idx 
        : unsortedListIndexMap.GetValueOrDefault(new DefaultSaberValue(), 0);

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
            list.AddRange(GetSortedData(filterOptions with { Favourites = true})
                .Select(meta => new ListInfoCellInfo(meta, favouritesManager.IsFavourite(meta.SaberFile))));
            
            PopulateValueIndexMap();
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
        
        list.AddRange(GetSortedData(filterOptions)
            .Where(m => m.SaberFile.FileInfo.DirectoryName == saberFoldersManager.CurrentDirectory.FullName)
            .Select(m => new ListInfoCellInfo(m, favouritesManager.IsFavourite(m.SaberFile))));
        
        PopulateValueIndexMap();
        return list;
        
        

        void PopulateValueIndexMap() => list.ForEach((cell, i) =>
        {
            if (cell.TryGetSaberValue(out var v)) valueIndexMap.Add(v, i);
        });
    }

    private IEnumerable<CustomSaberMetadata> GetSortedData(SaberListFilterOptions options)
    {
        var data = saberMetadataCache.GetRefreshedMetadata();
        
        if (options.Trails)
        {
            data = data.Where(meta => meta.HasTrails);
        }

        if (options.Favourites)
        {
            data = data.Where(meta => favouritesManager.IsFavourite(meta.SaberFile));
        }

        if (!string.IsNullOrWhiteSpace(options.SearchFilter))
        {
            data = data.Where(meta =>
                meta.Descriptor.SaberName.Contains(options.SearchFilter, StringComparison.CurrentCultureIgnoreCase)
                || meta.Descriptor.AuthorName.Contains(options.SearchFilter, StringComparison.CurrentCultureIgnoreCase));
        }
        
        data = options.OrderBy switch
        {
            OrderBy.Name => data.OrderBy(x => x.Descriptor.SaberName).ThenBy(x => x.Descriptor.AuthorName),
            OrderBy.Author => data.OrderBy(x => x.Descriptor.AuthorName).ThenBy(x => x.Descriptor.SaberName),
            OrderBy.RecentlyAdded => data.OrderBy(x => x.SaberFile.DateAdded).ThenBy(x => x.Descriptor.SaberName),
            _ => throw new ArgumentOutOfRangeException(nameof(options.OrderBy))
        };
        
        return options.ReverseOrder ? data.Reverse() : data;
    }
        
    private static bool TrySelect(List<IListCellInfo> list, int row, [NotNullWhen(true)] out IListCellInfo? cell) =>
        (cell = list.ElementAtOrDefault(row)) != null;
}
