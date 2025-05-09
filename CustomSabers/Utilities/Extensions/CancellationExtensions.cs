using System.Threading;

namespace CustomSabersLite.Utilities.Extensions;

internal static class CancellationExtensions
{
    public static void CancelThenDispose(this CancellationTokenSource source)
    {
        source.Cancel();
        source.Dispose();
    }
}