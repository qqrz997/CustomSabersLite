using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomSabersLite.Utilities.Extensions;

namespace CustomSabersLite.Menu.Views;

internal class CustomListCollection : IEnumerable<object>
{
    private readonly List<object> data = [];
    
    public int Count => data.Count;
    
    public void Clear() => data.Clear();
    public void Add(object item) => data.Add(item);
    public void AddRange(IEnumerable<object> items) => items.ForEach(data.Add);
    public object ElementAt(int index) => data.ElementAt(index);
    
    public IEnumerator<object> GetEnumerator() => data.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}