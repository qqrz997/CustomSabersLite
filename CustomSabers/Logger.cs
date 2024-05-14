using IPALogger = IPA.Logging.Logger;

namespace CustomSabersLite
{
    internal class Logger
    {
        private static IPALogger Log;

        internal Logger(IPALogger logger) => Log = logger;

        internal static void Debug(string message) => Log.Debug(message);

        internal static void Info(string message) => Log.Info(message);

        internal static void Warn(string message) => Log.Warn(message);

        internal static void Error(string message) => Log.Error(message);

        internal static void Critical(string message) => Log.Critical(message);
    }
}
