namespace LijsDev.CrystalReportsRunner.Core;
/// <summary>
/// Specify logger verbosity.
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Logs everything.
    /// </summary>
    Trace = 1,
    /// <summary>
    /// Logs Debug logs and higher levels.
    /// </summary>
    Debug = 2,
    /// <summary>
    /// Logs Info logs and higher levels.
    /// </summary>
    Info = 3,
    /// <summary>
    /// Logs Warning logs and higher levels.
    /// </summary>
    Warn = 4,
    /// <summary>
    /// Logs Error logs and higher levels.
    /// </summary>
    Error = 5,
    /// <summary>
    /// Only logs Fatal logs.
    /// </summary>
    Fatal = 6,
    /// <summary>
    /// Disables logging.
    /// </summary>
    Off = 0,
}
