using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CustomSabersLite.Utilities.Extensions;

namespace CustomSabersLite.Models;

internal class CustomListCollection : IEnumerable<ISaberListCell>
{
    private readonly List<ISaberListCell> data = [];
    
    public int Count => data.Count;
    
    public void Clear() => data.Clear();
    
    public void Add(ISaberListCell item) => data.Add(item);
    
    public void AddRange(IEnumerable<ISaberListCell> items) => items.ForEach(data.Add);
    
    public bool TryGetElementAt(int index, [NotNullWhen(true)] out ISaberListCell? saberListCell)
    {
        saberListCell = data.ElementAtOrDefault(index);
        return saberListCell != null;
    }

    public IEnumerator<ISaberListCell> GetEnumerator() => data.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}