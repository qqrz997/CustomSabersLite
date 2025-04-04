using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomSabersLite.Utilities.Extensions;

internal static class CoroutineStarterExtensions
{
    public static IEnumerable<Coroutine> StartCoroutines(
        this ICoroutineStarter coroutineStarter, params IEnumerable<IEnumerator> coroutines) =>
        coroutines.Select(coroutineStarter.StartCoroutine);
}