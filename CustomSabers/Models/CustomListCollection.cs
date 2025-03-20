using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CustomSabersLite.Utilities.Extensions;

namespace CustomSabersLite.Models;

internal class CustomListCollection : IEnumerable<IListCellInfo>
{
    private readonly List<IListCellInfo> data = [];
    
    public int Count => data.Count;
    
    public void Clear() => data.Clear();
    
    public void Add(IListCellInfo item) => data.Add(item);
    
    public void AddRange(IEnumerable<IListCellInfo> items) => items.ForEach(data.Add);
    
    public bool TryGetElementAt(int index, [NotNullWhen(true)] out IListCellInfo? saberListCell) => 
        (saberListCell = data.ElementAtOrDefault(index)) != null;

    public IEnumerator<IListCellInfo> GetEnumerator() => data.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}