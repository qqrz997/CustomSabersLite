using IPALogger = IPA.Logging.Logger;

namespace CustomSabersLite
{
    internal class Logger
    {
        internal static IPALogger Log { private get; set; }

        internal static void Debug(string message)
        {
            Log.Debug(message);
        }

        internal static void Info(string message)
        {
            Log.Info(message);
        }

        internal static void Warn(string message)
        {
            Log.Warn(message);
        }

        internal static void Error(string message)
        {
            Log.Error(message);
        }
    }
}
