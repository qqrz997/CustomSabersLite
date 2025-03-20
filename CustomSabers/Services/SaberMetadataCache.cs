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
        var cache = RefreshCache();
        
        if (options.Trails)
        {
            cache = cache.Where(meta => meta.HasTrails);
        }

        if (options.Favourites)
        {
            cache = cache.Where(meta => meta.IsFavourite);
        }

        if (!string.IsNullOrWhiteSpace(options.SearchFilter))
        {
            cache = cache.Where(meta =>
                meta.Descriptor.SaberName.Contains(options.SearchFilter)
                || meta.Descriptor.AuthorName.Contains(options.SearchFilter));
        }
        
        cache = options.OrderBy switch
        {
            OrderBy.Name => cache.OrderBy(x => x.Descriptor.SaberName).ThenBy(x => x.Descriptor.AuthorName),
            OrderBy.Author => cache.OrderBy(x => x.Descriptor.AuthorName).ThenBy(x => x.Descriptor.SaberName),
            OrderBy.RecentlyAdded => cache.OrderBy(x => x.SaberFile.DateAdded).ThenBy(x => x.Descriptor.SaberName),
            _ => throw new ArgumentOutOfRangeException(nameof(options.OrderBy))
        };
        
        return options.ReverseOrder ? cache.Reverse() : cache;
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