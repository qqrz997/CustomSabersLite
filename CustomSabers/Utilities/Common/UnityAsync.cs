using System;
using System.Threading.Tasks;
using IPA.Utilities.Async;

namespace CustomSabersLite.Utilities.Common;

internal static class UnityAsync
{
    public static Task<TResult> StartUnitySafeTask<TResult>(Func<TResult> lambda) =>
        UnityMainThreadTaskScheduler.Factory.StartNew(lambda);
}