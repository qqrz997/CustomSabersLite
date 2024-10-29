using System;
using IPALogger = IPA.Logging.Logger;
using Level = IPA.Logging.Logger.Level;

namespace CustomSabersLite;

internal class Logger
{
    private static IPALogger? IPALogger { get; set; }

    internal static void SetLogger(IPALogger logger) => IPALogger ??= logger;

    internal static void Trace(string? message) => Log(message, Level.Trace);

    internal static void Debug(object? message) => Log(message, Level.Debug);

    internal static void Info(object? message) => Log(message, Level.Info);

    internal static void Notice(object? message) => Log(message, Level.Notice);

    internal static void Warn(object? message) => Log(message, Level.Warning);

    internal static void Error(object? message) => Log(message, Level.Error);

    internal static void Critical(object? message) => Log(message, Level.Critical);

    private static void Log(object? message, Level level)
    {
        if (IPALogger is null)
        {
            return;
        }

        Action<string> func = level switch
        {
            Level.Trace => IPALogger.Trace,
            Level.Debug => IPALogger.Debug,
            Level.Info => IPALogger.Info,
            Level.Notice => IPALogger.Notice,
            Level.Warning => IPALogger.Warn,
            Level.Error => IPALogger.Error,
            _ => IPALogger.Critical,
        };
        message ??= "null";
        func(message.ToString());
    }
}
