using CustomSabersLite.Models;
using CustomSabersLite.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CustomSabersLite.UI.Managers;

internal class SaberListManager(PluginDirs dirs)
{
    private readonly PluginDirs directories = dirs;

    private List<SaberListCellInfo> Data { get; set; } = [];
    private List<SaberListCellInfo> SaberList { get; } = [];

    public void SetData(IEnumerable<CustomSaberMetadata> data) =>
        Data = data.Select(MetaToInfo).ToList();

    public List<SaberListCellInfo> GetList(SaberListFilterOptions filterOptions)
    {
        var sortedData = Data
            .OrderBy(i => filterOptions.OrderBy switch
        {
            OrderBy.Name => i.Metadata.Descriptor.SaberName, // should sort by a sanitized string / type for name or extension method
            OrderBy.Author => i.Metadata.Descriptor.AuthorName,
            _ => throw new NotImplementedException()
        }).ToList();

        SaberList.Clear();
        SaberList.Add(CellInfoForDefaultSabers);
        SaberList.AddRange(sortedData);

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

        if (Data.FirstOrDefault(i => i.Metadata.FileInfo.RelativePath == relativePath) is SaberListCellInfo i)
            Data.Remove(i);

        return true;
    }

    public int IndexForPath(string? relativePath) =>
        string.IsNullOrEmpty(relativePath) ? 0
        : SaberList.FirstOrDefault(i => i.Metadata?.FileInfo?.RelativePath == relativePath) is not SaberListCellInfo i ? 0
        : SaberList.IndexOf(i);

    public string? PathForIndex(int row) =>
        SaberList.ElementAtOrDefault(row) is SaberListCellInfo i ? i.Metadata.FileInfo.RelativePath : null;

    private static SaberListCellInfo CellInfoForDefaultSabers =>
        MetaToInfo(new CustomSaberMetadata(
            new(null, CustomSaberType.Default), 
            SaberLoaderError.None, 
            new("Default", "Beat Games", null),
            new(false, [])));

    private static SaberListCellInfo MetaToInfo(CustomSaberMetadata meta) =>
        new(meta, GetCellInfo(meta), GetCellIcon(meta));

    private static SaberListCellText GetCellInfo(CustomSaberMetadata meta) => meta.LoaderError switch
    {
        SaberLoaderError.None => new(meta.Descriptor.SaberName, meta.Descriptor.AuthorName),
        SaberLoaderError.InvalidFileType => new($"<color=#F77>Error - </color> {meta.FileInfo.FileName}", "File is not of a valid type"),
        SaberLoaderError.FileNotFound => new($"<color=#F77>Error - </color> {meta.FileInfo.FileName}", "Couldn't find file (was it deleted?)"),
        SaberLoaderError.LegacyWhacker => new($"<color=#F77>Not loaded - </color> {meta.FileInfo.FileName}", "Legacy whacker, incompatible with PC"),
        SaberLoaderError.NullBundle => new($"<color=#F77>Error - </color> {meta.FileInfo.FileName}", "Problem encountered when loading asset"),
        SaberLoaderError.NullAsset => new($"<color=#F77>Error - </color> {meta.FileInfo.FileName}", "Problem encountered when loading saber model"),
        _ => new($"<color=#F77>Error - </color> {meta.FileInfo.FileName}", "Unknown error encountered during loading")
    };

    private static IThumbnail GetCellIcon(CustomSaberMetadata meta) => meta.Descriptor.Image switch
    {
        _ when meta.Descriptor.SaberName == "Default" => 
            ImageUtils.defaultCoverImage == null ? new NoThumbnail() : new SpriteThumbnail(ImageUtils.defaultCoverImage), // amazing stuff really
        [..] bytes => new ThumbnailWithData(bytes),
        _ => ImageUtils.nullCoverImage == null ? new NoThumbnail() : new SpriteThumbnail(ImageUtils.nullCoverImage)
    };
}
