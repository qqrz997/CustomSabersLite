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

    private IQueryable<SaberListCellInfo> Data { get; set; } = Array.Empty<SaberListCellInfo>().AsQueryable();
    private List<SaberListCellInfo> SaberList { get; set; } = [];

    private SaberListCellInfo InfoForDefaultSabers { get; } = MetaToInfo(CustomSaberMetadata.Default);

    public void SetData(IEnumerable<CustomSaberMetadata> data) => 
        Data = data.Select(MetaToInfo).AsQueryable();

    public List<SaberListCellInfo> GetList(SaberListFilterOptions? filterOptions = null)
    {
        filterOptions ??= SaberListFilterOptions.Default;

        // todo - this logic may not stay here
        var orderedData = filterOptions.OrderBy switch
        {
            OrderBy.Name => Data
                .OrderBy(i => i.Metadata.Descriptor.SaberName)
                .ThenBy(i => i.Metadata.Descriptor.AuthorName),

            OrderBy.Author => Data
                .OrderBy(i => i.Metadata.Descriptor.AuthorName)
                .ThenBy(i => i.Metadata.Descriptor.SaberName),

            _ => throw new NotImplementedException()
        };

        return SaberList = [InfoForDefaultSabers, .. orderedData];
    }

    public bool DeleteSaber(string relativePath)
    {
        var currentSaberPath = Path.Combine(PluginDirs.CustomSabers.FullName, relativePath);

        if (!File.Exists(currentSaberPath))
            return false;

        var destinationPath = Path.Combine(PluginDirs.DeletedSabers.FullName, relativePath);

        if (File.Exists(destinationPath))
            File.Delete(destinationPath);

        File.Move(currentSaberPath, destinationPath);

        var deletedData = Data.FirstOrDefault(i => i.Metadata.FileInfo.RelativePath == relativePath);
        Data = Data.Where(i => i != deletedData);
        return deletedData != null;
    }

    public int IndexForPath(string? relativePath) =>
        string.IsNullOrEmpty(relativePath) ? 0
        : SaberList.FirstOrDefault(i => i.Metadata.FileInfo.RelativePath == relativePath) is not SaberListCellInfo i ? 0
        : SaberList.IndexOf(i);

    public SaberListCellInfo? Select(int row) =>
        SaberList.ElementAtOrDefault(row);

    private static SaberListCellInfo MetaToInfo(CustomSaberMetadata meta)
    {
        var text = SaberListCellText.Create(meta);
        var spriteIcon = meta.LoaderError switch
        {
            SaberLoaderError.None => meta.Descriptor.Icon,
            _ => CSLResources.DefaultCoverImage
        };
        return new SaberListCellInfo(meta, text, spriteIcon);
    }
}
