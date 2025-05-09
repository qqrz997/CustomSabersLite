﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomSabersLite.Utilities.Extensions;

internal static class LinqExtensions
{
    public static void ForEach<T>(this IEnumerable<T> seq, Action<T> action)
    {
        foreach (var item in seq) action(item);
    }

    public static async Task ForEach<T>(this IEnumerable<T> seq, Func<T, Task> asyncAction)
    {
        foreach (var item in seq) await asyncAction(item);
    }

    public static void ForEach<T>(this IEnumerable<T> seq, Action<T, int> action)
    {
        int index = -1;
        foreach (var item in seq)
        {
            checked { index++; }
            action(item, index);
        }
    }

    public static IEnumerable<T> WhereNot<T>(this IEnumerable<T> seq, Func<T, bool> predicate) =>
        seq.Where(x => predicate(x) == false);
}
