using CustomSabersLite.Components.Managers;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomSabersLite.UI.Managers;

internal class SaberListManager(SaberInstanceManager saberInstances)
{
    private readonly SaberInstanceManager saberInstanceManager = saberInstances;

    private List<SaberListCellInfo> Data { get; set; } = [];
    private List<SaberListCellInfo> SaberList { get; set; } = [];

    private SaberListCellInfo InfoForDefaultSabers { get; } = MetaToInfo(CustomSaberMetadata.Default);

    public event Action? SaberListUpdated;

    public IEnumerable<SaberListCellInfo> List => SaberList;

    public void SetData(IEnumerable<CustomSaberMetadata> data)
    {
        Data = data.Select(MetaToInfo).ToList();
        SaberListUpdated?.Invoke();
    }

    public void Sort(SaberListFilterOptions? filterOptions = null)
    {
        filterOptions ??= SaberListFilterOptions.Default;

        var filtered = string.IsNullOrEmpty(filterOptions.SearchFilter) ? Data
            : Data.Where(i => i.Contains(filterOptions.SearchFilter));

        var ordererdData = filterOptions.OrderBy switch
        {
            OrderBy.Name => filtered.OrderBy(i => i.Metadata.Descriptor.SaberName).ThenBy(i => i.Metadata.Descriptor.AuthorName),
            OrderBy.Author => filtered.OrderBy(i => i.Metadata.Descriptor.AuthorName).ThenBy(i => i.Metadata.Descriptor.SaberName),
            OrderBy.RecentlyAdded => filtered.OrderByDescending(i => i.Metadata.FileInfo.DateAdded),
            _ => throw new NotImplementedException()
        };

        SaberList = [InfoForDefaultSabers, ..ordererdData];
    }

    public bool DeleteSaber(string? relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
            return false;

        var currentSaberPath = Path.Combine(PluginDirs.CustomSabers.FullName, relativePath);

        if (!File.Exists(currentSaberPath))
            return false;

        var destinationPath = Path.Combine(PluginDirs.DeletedSabers.FullName, relativePath);

        if (File.Exists(destinationPath))
            File.Delete(destinationPath);

        saberInstanceManager.TryGetSaber(relativePath)?.Dispose(true);
        File.Move(currentSaberPath, destinationPath);

        var deletedInfo = Data.FirstOrDefault(i => i.Metadata.FileInfo.RelativePath == relativePath);
        Data.Remove(deletedInfo);

        return deletedInfo != null;
    }

    public int IndexForPath(string? relativePath) =>
        string.IsNullOrEmpty(relativePath) ? 0
        : SaberList.FirstOrDefault(i => i.Metadata.FileInfo.RelativePath == relativePath) is not SaberListCellInfo i ? 0
        : SaberList.IndexOf(i);

    public SaberListCellInfo? Select(int row) =>
        SaberList.ElementAtOrDefault(row);

    public bool Contains(string? relativePath) =>
        !string.IsNullOrEmpty(relativePath) 
        && Data.FirstOrDefault(i => i.Metadata.FileInfo.RelativePath == relativePath) is not null;

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
