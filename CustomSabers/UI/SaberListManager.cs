using CustomSabersLite.Models;
using CustomSabersLite.Utilities;
using CustomSabersLite.Utilities.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomSabersLite.UI;

internal class SaberListManager(SaberInstanceManager saberInstances)
{
    private readonly SaberInstanceManager saberInstanceManager = saberInstances;
 
    private List<SaberListCellInfo> Data { get; set; } = [];
    private List<SaberListCellInfo> SaberList { get; set; } = [];

    private SaberListCellInfo InfoForDefaultSabers { get; } = MetaToInfo(CustomSaberMetadata.Default);

    public void Clear() => Data.Clear();
    public void SetData(IEnumerable<CustomSaberMetadata> data) => Data = data.Select(MetaToInfo).ToList();

    public IEnumerable<SaberListCellInfo> UpdateList(SaberListFilterOptions filterOptions) =>
        SaberList = GetSortedData(filterOptions).Prepend(InfoForDefaultSabers).ToList();

    public bool DeleteSaber(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            return false;
        }

        string currentSaberPath = Path.Combine(PluginDirs.CustomSabers.FullName, relativePath);
        if (!File.Exists(currentSaberPath))
        {
            return false;
        }

        string destinationPath = Path.Combine(PluginDirs.DeletedSabers.FullName, relativePath);
        if (File.Exists(destinationPath))
        {
            File.Delete(destinationPath);
        }

        saberInstanceManager.TryGetSaber(relativePath)?.Dispose(true);
        File.Move(currentSaberPath, destinationPath);

        var deletedInfo = Data.FirstOrDefault(i => i.Metadata.SaberFile.RelativePath == relativePath);
        Data.Remove(deletedInfo);

        return deletedInfo != null;
    }

    public int IndexForPath(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath)) return 0;
        int index = SaberList.IndexOf(SaberList.FirstOrDefault(i => i.Metadata.SaberFile.RelativePath == relativePath));
        return index != -1 ? index : 0;
    }

    public SaberListCellInfo? Select(int row) =>
        SaberList.ElementAtOrDefault(row);

    public bool CurrentListContains(string? relativePath) =>
        !string.IsNullOrEmpty(relativePath)
        && SaberList.FirstOrDefault(i => i.Metadata.SaberFile.RelativePath == relativePath) is not null;

    private IEnumerable<SaberListCellInfo> GetSortedData(SaberListFilterOptions filterOptions)
    {
        var filtered = string.IsNullOrEmpty(filterOptions.SearchFilter) ? Data
            : Data.Where(i => i.Contains(filterOptions.SearchFilter));

        var orderedData = filterOptions.OrderBy switch
        {
            OrderBy.Name => filtered.OrderBy(i => i.Metadata.Descriptor.SaberName).ThenBy(i => i.Metadata.Descriptor.AuthorName),
            OrderBy.Author => filtered.OrderBy(i => i.Metadata.Descriptor.AuthorName).ThenBy(i => i.Metadata.Descriptor.SaberName),
            OrderBy.RecentlyAdded => filtered.OrderByDescending(i => i.Metadata.SaberFile.DateAdded),
            _ => throw new NotImplementedException()
        };

        return filterOptions.ReverseOrder ? orderedData.Reverse() : orderedData;
    }

    private static SaberListCellInfo MetaToInfo(CustomSaberMetadata meta)
    {
        var text = new SaberListCellText(meta);
        var spriteIcon = meta.LoaderError switch
        {
            SaberLoaderError.None => meta.Descriptor.Icon,
            _ => CSLResources.DefaultCoverImage
        };
        return new SaberListCellInfo(meta, text, spriteIcon);
    }
}
