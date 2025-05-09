using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CustomSabersLite.Models;

namespace CustomSabersLite.Services;

internal class SaberMetadataCache
{
    private readonly Dictionary<string, CustomSaberMetadata> cache = [];

    public bool TryAdd(CustomSaberMetadata saberMetadata) => cache.TryAdd(saberMetadata.SaberFile.Hash, saberMetadata);

    public void Remove(string saberHash) => cache.Remove(saberHash);

    public void Clear() => cache.Clear();

    public bool TryGetMetadata(string? saberHash, [NotNullWhen(true)] out CustomSaberMetadata? meta) => 
        (meta = GetOrDefault(saberHash)) != null;

    public CustomSaberMetadata? GetOrDefault(string? saberHash)
    {
        if (saberHash is null || !cache.TryGetValue(saberHash, out var meta))
        {
            return null;
        }
        
        meta.SaberFile.FileInfo.Refresh();
        
        if (!meta.SaberFile.FileInfo.Exists)
        {
            Remove(saberHash);
            return null;
        }
        
        return meta;
    }

    public IEnumerable<CustomSaberMetadata> GetSortedData(SaberListFilterOptions options)
    {
        var data = RefreshCache();
        
        if (options.Trails)
        {
            data = data.Where(meta => meta.HasTrails);
        }

        if (options.Favourites)
        {
            data = data.Where(meta => meta.IsFavourite);
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

    private IEnumerable<CustomSaberMetadata> RefreshCache()
    {
        foreach (var meta in cache.Values)
        {
            meta.SaberFile.FileInfo.Refresh();
        }
        
        return cache.Values;
    }
}