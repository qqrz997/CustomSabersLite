using CustomSabersLite.Components.Managers;
using CustomSabersLite.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomSabersLite.UI.Managers;

internal class SaberListManager(SaberInstanceManager saberInstances)
{
    private readonly SaberInstanceManager saberInstanceManager = saberInstances;

    private List<SaberListCellInfo> Data { get; } = [];
    private List<SaberListCellInfo> SaberList { get; } = [];

    private SaberListCellInfo InfoForDefaultSabers { get; } = MetaToInfo(CustomSaberMetadata.Default);

    public void SetData(IEnumerable<CustomSaberMetadata> data)
    {
        Data.Clear();
        Data.AddRange(data.Select(MetaToInfo));
    }

    public List<SaberListCellInfo> GetList(SaberListFilterOptions? filterOptions = null)
    {
        filterOptions ??= SaberListFilterOptions.Default;

        // this logic feels like it doesn't belong here and has no actual structure/meaning
        // it will not easily expand
        var sortedData = Data
            .OrderBy(i => filterOptions.OrderBy switch
        {
            OrderBy.Name => i.Metadata.Descriptor.SaberName,
            OrderBy.Author => i.Metadata.Descriptor.AuthorName,
            _ => throw new NotImplementedException()
        })
            .ThenBy(i => filterOptions.OrderBy switch
        {
            OrderBy.Name => i.Metadata.Descriptor.AuthorName,
            OrderBy.Author => i.Metadata.Descriptor.SaberName,
            _ => throw new NotImplementedException()
        });

        SaberList.Clear();
        SaberList.Add(InfoForDefaultSabers);
        SaberList.AddRange(sortedData);

        return SaberList;
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

        return Data.Remove(Data.FirstOrDefault(i => i.Metadata.FileInfo.RelativePath == relativePath));
    }

    public int IndexForPath(string? relativePath) =>
        string.IsNullOrEmpty(relativePath) ? 0
        : SaberList.FirstOrDefault(i => i.Metadata.FileInfo.RelativePath == relativePath) is not SaberListCellInfo i ? 0
        : SaberList.IndexOf(i);

    public SaberListCellInfo? Select(int row) =>
        SaberList.ElementAtOrDefault(row);

    private static SaberListCellInfo MetaToInfo(CustomSaberMetadata meta) =>
        new(meta, GetCellInfo(meta), meta.Descriptor.Icon);

    private static SaberListCellText GetCellInfo(CustomSaberMetadata meta) => meta.LoaderError switch
    {
        SaberLoaderError.None => new(meta.Descriptor.SaberName.FullName, meta.Descriptor.AuthorName.FullName),
        SaberLoaderError.InvalidFileType => new($"<color=#F77>Error - </color> {meta.FileInfo.FileName}", "File is not of a valid type"),
        SaberLoaderError.FileNotFound => new($"<color=#F77>Error - </color> {meta.FileInfo.FileName}", "Couldn't find file"),
        SaberLoaderError.LegacyWhacker => new($"<color=#F77>Not loaded - </color> {meta.FileInfo.FileName}", "Legacy whacker, incompatible with PC"),
        SaberLoaderError.NullBundle => new($"<color=#F77>Error - </color> {meta.FileInfo.FileName}", "Problem encountered when loading asset"),
        SaberLoaderError.NullAsset => new($"<color=#F77>Error - </color> {meta.FileInfo.FileName}", "Problem encountered when loading saber model"),
        _ => new($"<color=#F77>Error - </color> {meta.FileInfo.FileName}", "Unknown error encountered during loading")
    };
}
