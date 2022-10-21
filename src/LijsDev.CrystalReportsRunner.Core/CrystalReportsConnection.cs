#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

namespace LijsDev.CrystalReportsRunner.Core;

/// <summary>
/// Crystal Report Report Connection
/// </summary>
public class CrystalReportsConnection
{
    /// <summary>
    /// Maps to Crystal Reports ConnectionInfo -> ServerName
    /// </summary>
    public string Server { get; set; }

    /// <summary>
    /// Maps to Crystal Reports ConnectionInfo -> DatabaseName
    /// </summary>
    public string Database { get; set; }

    /// <summary>
    /// Maps to Crystal Reports ConnectionInfo -> UserID
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// Maps to Crystal Reports ConnectionInfo -> Password
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Maps to Crystal Reports ConnectionInfo -> IntegratedSecurity
    /// </summary>
    public bool UseIntegratedSecurity { get; set; }

    /// <summary>
    /// Logon Properties
    /// </summary>
    public Dictionary<string, string> LogonProperties { get; set; }
}
