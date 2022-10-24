namespace LijsDev.CrystalReportsRunner.Core;

using NLog;

/// <summary>
/// NLog Helper methods.
/// </summary>
public static class NLogHelper
{
    /// <summary>
    /// Configures NLog according to the provided command line options.
    /// </summary>
    public static void ConfigureNLog(string? logPath, LogLevel logLevel)
    {
        var level = ToNLog(logLevel);

        // from: https://gist.github.com/pmullins/f21c3d83e96b9fd8a720
        if (level == NLog.LogLevel.Off)
        {
            LogManager.SuspendLogging();
        }
        else
        {
            if (!LogManager.IsLoggingEnabled())
            {
                LogManager.ResumeLogging();
            }

            LogManager.Configuration ??= new();

            foreach (var rule in LogManager.Configuration.LoggingRules)
            {
                for (var i = 0; i < level.Ordinal; i++)
                {
                    rule.DisableLoggingForLevel(NLog.LogLevel.FromOrdinal(i));
                }

                // Iterate over all levels up to and including the target, (re)enabling them.
                for (var i = level.Ordinal; i <= 5; i++)
                {
                    rule.EnableLoggingForLevel(NLog.LogLevel.FromOrdinal(i));
                }
            }
        }

        if (!string.IsNullOrEmpty(logPath))
        {
            // Change logfile location
            var target = LogManager.Configuration.FindTargetByName("logfile") as NLog.Targets.FileTarget;
            if (target is not null)
            {
                if (string.IsNullOrEmpty(Path.GetExtension(logPath)))
                {
                    target.FileName = Path.Combine(logPath, "${processname}-${shortdate}.log");
                }
                else
                {
                    target.FileName = logPath;
                }
            }
        }

        LogManager.ReconfigExistingLoggers();
    }

    private static NLog.LogLevel ToNLog(LogLevel level) => level switch
    {
        LogLevel.Trace => NLog.LogLevel.Trace,
        LogLevel.Debug => NLog.LogLevel.Debug,
        LogLevel.Info => NLog.LogLevel.Info,
        LogLevel.Warn => NLog.LogLevel.Warn,
        LogLevel.Error => NLog.LogLevel.Error,
        LogLevel.Fatal => NLog.LogLevel.Fatal,
        LogLevel.Off => NLog.LogLevel.Off,
        _ => throw new NotImplementedException(),
    };

}
