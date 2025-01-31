using System.Collections.Generic;
using System.IO;
using System.Linq;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities.Common;
using CustomSabersLite.Utilities.Services;

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

    private readonly List<SaberListCellInfo> saberList = [];
    private readonly SaberListCellInfo defaultSabersInfo = new("Default", "Beat Games", CSLResources.DefaultCoverImage);

    public void Clear() => saberList.Clear();

    public IEnumerable<SaberListCellInfo> GetUnsortedData() => 
        saberMetadataCache.GetSortedData(SaberListFilterOptions.Default);
    
    public IEnumerable<SaberListCellInfo> UpdateList(SaberListFilterOptions filterOptions)
    {
        saberList.Clear();
        saberList.Add(defaultSabersInfo);
        saberList.AddRange(saberMetadataCache.GetSortedData(filterOptions));
        return saberList;
    }

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

        if (saberPrefabCache.TryGetSaberPrefab(saberFile.Hash, out var saberData))
        {
            saberData.Dispose(true);
        }

        saberFile.FileInfo.MoveTo(destinationFile.FullName);
        saberMetadataCache.Remove(saberFile.Hash);
    }

    public int IndexForSaberHash(string? saberHash)
    {
        if (string.IsNullOrWhiteSpace(saberHash)) return 0;
        int index = saberList.IndexOf(saberList.FirstOrDefault(i => i.SaberHash == saberHash));
        return index != -1 ? index : 0;
    }

    public SaberListCellInfo? SelectFromUnsortedData(int row) =>
        saberMetadataCache.GetSortedData(SaberListFilterOptions.Default).ElementAtOrDefault(row);
    
    public SaberListCellInfo? SelectFromCurrentList(int row) =>
        saberList.ElementAtOrDefault(row);

    public bool CurrentListContains(string? saberHash) =>
        !string.IsNullOrEmpty(saberHash) && saberList.Any(i => i.SaberHash == saberHash);
}
