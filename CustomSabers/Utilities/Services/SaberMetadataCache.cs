using System;
using System.Collections.Generic;
using System.Linq;
using CustomSabersLite.Models;

namespace CustomSabersLite.Utilities.Services;

internal class SaberMetadataCache
{
    private readonly Dictionary<string, CustomSaberMetadata> cache = [];

    public bool TryAdd(CustomSaberMetadata saberMetadata) => cache.TryAdd(saberMetadata.SaberFile.Hash, saberMetadata);

    public void Remove(string saberHash) => cache.Remove(saberHash);

    public CustomSaberMetadata? GetOrDefault(string saberHash) => cache.GetValueOrDefault(saberHash);
    
    public IEnumerable<SaberListCellInfo> GetSortedData(SaberListFilterOptions options)
    {
        // TODO: remove metadata for sabers that don't exist. this may happen if the file gets move/deleted
        var cachedData = options.SaberListType switch
        {
            SaberListType.Trails => cache.Values.Where(meta => meta.TrailsInfo.HasCustomSaberTrail),
            _ => cache.Values
        };
        
        var data = cachedData.Select(meta => new SaberListCellInfo(meta));

        if (!string.IsNullOrWhiteSpace(options.SearchFilter))
        {
            data = data.Where(info => info.TextContains(options.SearchFilter));
        }
            
        data = options.OrderBy switch
        {
            OrderBy.Name => data.OrderBy(i => i.NameText).ThenBy(i => i.AuthorText),
            OrderBy.Author => data.OrderBy(i => i.AuthorText).ThenBy(i => i.NameText),
            OrderBy.RecentlyAdded => data.OrderBy(i => i.DateAdded).ThenBy(i => i.NameText),
            _ => throw new ArgumentOutOfRangeException(nameof(options.OrderBy))
        };

        return options.ReverseOrder ? data.Reverse() : data;
    }
}