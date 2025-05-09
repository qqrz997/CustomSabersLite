using System;
using IPALogger = IPA.Logging.Logger;
using Level = IPA.Logging.Logger.Level;

namespace CustomSabersLite;

internal class Logger
{
    private static IPALogger? IpaLogger { get; set; }

    internal static void SetLogger(IPALogger logger) => IpaLogger ??= logger;

    internal static void Trace(string? message) => Log(message, Level.Trace);
    internal static void Debug(object? message) => Log(message, Level.Debug);
    internal static void Info(object? message) => Log(message, Level.Info);
    internal static void Notice(object? message) => Log(message, Level.Notice);
    internal static void Warn(object? message) => Log(message, Level.Warning);
    internal static void Error(object? message) => Log(message, Level.Error);
    internal static void Critical(object? message) => Log(message, Level.Critical);

    private static void Log(object? message, Level level)
    {
        if (IpaLogger is null) return;
        
        Action<string> action = level switch
        {
            Level.Trace => IpaLogger.Trace,
            Level.Debug => IpaLogger.Debug,
            Level.Info => IpaLogger.Info,
            Level.Notice => IpaLogger.Notice,
            Level.Warning => IpaLogger.Warn,
            Level.Error => IpaLogger.Error,
            Level.Critical => IpaLogger.Critical,
            Level.None => throw new NotImplementedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(level))
        };
        
        action(message?.ToString() ?? "null");
    }
}
