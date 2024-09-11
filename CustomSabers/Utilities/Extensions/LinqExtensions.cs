using System;
using System.Collections.Generic;

namespace CustomSabersLite.Utilities;

internal static class LinqExtensions
{
    public static void ForEach<T>(this IEnumerable<T> seq, Action<T> action)
    {
        foreach (var item in seq) action(item);
    }
}
