using System;
using System.Collections.Generic;
using System.Linq;
using CustomSabersLite.Models;

namespace CustomSabersLite.Utilities.Services;

internal class SaberMetadataCache
{
    private readonly Dictionary<string, CustomSaberMetadata> cache = [];

    public bool TryAdd(ISaberMetadata saberMetadata) =>
        saberMetadata is CustomSaberMetadata customSaberMetadata 
        && cache.TryAdd(customSaberMetadata.SaberFile.Hash, customSaberMetadata);

    public void Remove(string saberHash) => cache.Remove(saberHash);

    public CustomSaberMetadata? GetOrDefault(string saberHash) => cache.GetValueOrDefault(saberHash);
    
    public IEnumerable<SaberListCellInfo> GetSortedData(SaberListFilterOptions options)
    {
        var data = cache.Values.Select(meta => new SaberListCellInfo(meta));

        if (!string.IsNullOrWhiteSpace(options.SearchFilter))
        {
            data = data.Where(info => info.TextContains(options.SearchFilter));
        }
            
        data = options.OrderBy switch
        {
            OrderBy.Name => data.OrderBy(i => i.Text).ThenBy(i => i.Subtext),
            OrderBy.Author => data.OrderBy(i => i.Subtext).ThenBy(i => i.Text),
            OrderBy.RecentlyAdded => data.OrderBy(i => i.DateAdded).ThenBy(i => i.Text),
            _ => throw new ArgumentOutOfRangeException(nameof(options.OrderBy))
        };

        return options.ReverseOrder ? data.Reverse() : data;
    }
}