using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    
    public SaberListManager(SaberPrefabCache saberPrefabCache, SaberMetadataCache saberMetadataCache)
    {
        this.saberPrefabCache = saberPrefabCache;
        this.saberMetadataCache = saberMetadataCache;
    }

    private readonly List<SaberListCellInfo> list = [];

    public void Clear() => list.Clear();

    public IEnumerable<SaberListCellInfo> GetUnsortedData() => 
        saberMetadataCache.GetSortedData(SaberListFilterOptions.Default);

    public IEnumerable<SaberListCellInfo> UpdateList(SaberListFilterOptions filterOptions) =>
        filterOptions.SaberListType switch
        {
            SaberListType.Sabers => PopulateSaberList(filterOptions),
            SaberListType.Trails => PopulateTrailList(filterOptions),
            _ => throw new ArgumentOutOfRangeException(nameof(filterOptions.SaberListType))
        };

    public void DeleteSaber(string saberHash)
    {
        var saberFile = saberMetadataCache.GetOrDefault(saberHash)?.SaberFile;
        if (saberFile is null || !saberFile.FileInfo.Exists)
        {
            return;
        }

        var destinationFile = new FileInfo(Path.Combine(PluginDirs.DeletedSabers.FullName, saberFile.FileInfo.Name));
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

    public SaberListCellInfo? SelectFromUnsortedData(int row) =>
        saberMetadataCache.GetSortedData(SaberListFilterOptions.Default).ElementAtOrDefault(row);
    
    public SaberListCellInfo? SelectFromCurrentList(int row) => list.ElementAtOrDefault(row);

    public bool CurrentListContains(string? saberHash) =>
        !string.IsNullOrEmpty(saberHash) && list.Any(i => i.Value == saberHash);

    private IEnumerable<SaberListCellInfo> PopulateSaberList(SaberListFilterOptions filterOptions)
    {
        list.Clear();
        list.Add(new("Default", "Beat Games", DefaultCoverImage, null));
        list.AddRange(saberMetadataCache.GetSortedData(filterOptions));
        return list;
    }

    private IEnumerable<SaberListCellInfo> PopulateTrailList(SaberListFilterOptions filterOptions)
    {
        list.Clear();
        list.Add(new("Custom", "Use the trail of the selected saber", TrailDurationIcon, "custom"));
        list.Add(new("Default", "Beat Games", DefaultCoverImage, "default"));
        list.Add(new("None", "Use no trail", BlankSprite, null));
        list.AddRange(saberMetadataCache.GetSortedData(filterOptions));
        return list;
    }
}
