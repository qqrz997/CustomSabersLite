using CustomSabersLite.Data;
using CustomSabersLite.Models;
using CustomSabersLite.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace CustomSabersLite.UI.Managers;

internal class SaberListManager(PluginDirs dirs)
{
    private readonly PluginDirs directories = dirs;

    private SaberListCellInfo[] Data { get; set; } = [];
    private List<SaberListCellInfo> SaberList { get; } = [];

    public void SetData(IEnumerable<CustomSaberMetadata> data) =>
        Data = data.Select(MetaToInfo).ToArray();

    public List<SaberListCellInfo> GetList(SaberListFilterOptions filterOptions)
    {
        var sortedData = Data
            .OrderBy(i => filterOptions.OrderBy switch
        {
            OrderBy.Name => i.Metadata.SaberName, // should sort by a sanitized string (todo - separate the json model)
            OrderBy.Author => i.Metadata.AuthorName,
            OrderBy.Size or _ => throw new NotImplementedException()
        }).ToList();

        SaberList.Clear();
        SaberList.Add(CellInfoForDefaultSabers);
        SaberList.AddRange(sortedData);

        SaberList.ForEach(i => Logger.Warn(i.Metadata.SaberName));
        return SaberList;
    }

    public bool DeleteSaber(string relativePath)
    {
        var currentSaberPath = Path.Combine(directories.CustomSabers.FullName, relativePath);

        if (!File.Exists(currentSaberPath))
            return false;

        var destinationPath = Path.Combine(directories.DeletedSabers.FullName, relativePath);

        if (File.Exists(destinationPath))
            File.Delete(destinationPath);

        File.Move(currentSaberPath, destinationPath);
        SaberList.RemoveAt(IndexForPath(relativePath));

        return true;
    }

    public int IndexForPath(string relativePath) =>
        string.IsNullOrEmpty(relativePath) ? 0
        : SaberList.FirstOrDefault(i => i.Metadata.RelativePath == relativePath) is not SaberListCellInfo i ? 0
        : SaberList.IndexOf(i);

    public string PathForIndex(int row) =>
        SaberList.ElementAtOrDefault(row) is SaberListCellInfo i ? i.Metadata.RelativePath : null;

    private static SaberListCellInfo CellInfoForDefaultSabers => 
        MetaToInfo(DefaultMeta);

    private static CustomSaberMetadata DefaultMeta => 
        new() { SaberName = "Default", AuthorName = "Beat Games" }; // this is a model issue

    private static SaberListCellInfo MetaToInfo(CustomSaberMetadata meta) =>
        new(meta, GetCellInfo(meta), GetCellIcon(meta));

    private static SaberListCellText GetCellInfo(CustomSaberMetadata meta) => meta.LoadingError switch // again, this shouldn't be here
    {
        SaberLoaderError.None => new(meta.SaberName, meta.AuthorName),
        SaberLoaderError.InvalidFileType => new($"<color=#F77>Error - </color> {meta.SaberName}", "File is not of a valid type"),
        SaberLoaderError.FileNotFound => new($"<color=#F77>Error - </color> {meta.SaberName}", "Couldn't find file (was it deleted?)"),
        SaberLoaderError.LegacyWhacker => new($"<color=#F77>Not loaded - </color> {meta.SaberName}", "Legacy whacker, incompatible with PC"),
        SaberLoaderError.NullBundle => new($"<color=#F77>Error - </color> {meta.SaberName}", "Problem encountered when loading asset"),
        SaberLoaderError.NullAsset => new($"<color=#F77>Error - </color> {meta.SaberName}", "Problem encountered when loading saber model"),
        _ => new($"<color=#F77>Error - </color> {meta.SaberName}", "Unknown error encountered during loading")
    };

    private static IThumbnail GetCellIcon(CustomSaberMetadata meta) => meta.CoverImage switch
    {
        _ when meta.SaberName == "Default" && meta.AuthorName == "Beat Games" => new ThumbnailWithSprite(ImageUtils.defaultCoverImage), // amazing stuff
        [..] bytes  => new ThumbnailWithData(bytes),
        _ => new ThumbnailWithSprite(ImageUtils.nullCoverImage)
    };
}

internal enum OrderBy
{
    Name,
    Author,
    Size, // ?
}
