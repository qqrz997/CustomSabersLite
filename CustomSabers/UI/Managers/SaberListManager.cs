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

    private List<SaberListCellInfo> Data { get; } = [];
    private List<SaberListCellInfo> SaberList { get; } = [];

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
        : SaberList.FirstOrDefault(i => i.Metadata.FileInfo.RelativePath == relativePath) is not SaberListCellInfo i ? 0
        : SaberList.IndexOf(i);

    public SaberListCellInfo? Select(int row) =>
        SaberList.ElementAtOrDefault(row);

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
        SaberLoaderError.None => new(meta.Descriptor.SaberName.FullName, meta.Descriptor.AuthorName.FullName),
        SaberLoaderError.InvalidFileType => new($"<color=#F77>Error - </color> {meta.FileInfo.FileName}", "File is not of a valid type"),
        SaberLoaderError.FileNotFound => new($"<color=#F77>Error - </color> {meta.FileInfo.FileName}", "Couldn't find file (was it deleted?)"),
        SaberLoaderError.LegacyWhacker => new($"<color=#F77>Not loaded - </color> {meta.FileInfo.FileName}", "Legacy whacker, incompatible with PC"),
        SaberLoaderError.NullBundle => new($"<color=#F77>Error - </color> {meta.FileInfo.FileName}", "Problem encountered when loading asset"),
        SaberLoaderError.NullAsset => new($"<color=#F77>Error - </color> {meta.FileInfo.FileName}", "Problem encountered when loading saber model"),
        _ => new($"<color=#F77>Error - </color> {meta.FileInfo.FileName}", "Unknown error encountered during loading")
    };

    private static IThumbnail GetCellIcon(CustomSaberMetadata meta) => meta.Descriptor.Image switch
    {
        _ when meta.Descriptor.SaberName.FullName == "Default" => 
            ImageUtils.defaultCoverImage == null ? new NoThumbnail() : new SpriteThumbnail(ImageUtils.defaultCoverImage), // amazing stuff really
        [..] bytes => new ThumbnailWithData(bytes),
        _ => ImageUtils.nullCoverImage == null ? new NoThumbnail() : new SpriteThumbnail(ImageUtils.nullCoverImage)
    };
}
