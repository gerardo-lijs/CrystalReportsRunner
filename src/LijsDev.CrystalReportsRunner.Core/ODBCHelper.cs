namespace LijsDev.CrystalReportsRunner.Core;

/// <summary>
/// ODBC helper methods
/// </summary>
public static class ODBCHelper
{
    /// <summary>
    /// Creates an ODBC User DSN in Windows Registry using ODBC Driver 17 for SQL Server
    /// </summary>
    public static void UserDSN_SqlConnectionODBC_v17_Create(string dsnName, string server,
        bool useIntegratedSecurity = false,
        bool encrypt = false,
        bool trustServerCertificate = false)
    {
        var settings = ParseSqlConnectionODBC_Settings_v17(server, useIntegratedSecurity, encrypt, trustServerCertificate);
        UserDSN_Create(dsnName, settings);
    }

    /// <summary>
    /// Creates an ODBC User DSN in Windows Registry using ODBC Driver 18 for SQL Server
    /// </summary>
    public static void UserDSN_SqlConnectionODBC_v18_Create(string dsnName, string server,
        bool useIntegratedSecurity = false,
        SqlConnectionEncryptOption_v18 encryptOption = SqlConnectionEncryptOption_v18.Mandatory,
        bool trustServerCertificate = false,
        string? hostNameInCertificate = null)
    {
        var settings = ParseSqlConnectionODBC_Settings_v18(server, useIntegratedSecurity, encryptOption, trustServerCertificate, hostNameInCertificate);
        UserDSN_Create(dsnName, settings);
    }

    /// <summary>
    /// These options are used to control encryption behavior of the communication between the server and the client.
    /// </summary>
    public enum SqlConnectionEncryptOption_v18
    {
        /// <summary>
        /// Specifies that TLS encryption is optional when connecting to the server. If the server requires encryption, encryption will be negotiated.
        /// </summary>
        Optional = 0,
        /// <summary>
        /// Specifies that TLS encryption is required when connecting to the server. If the server doesn't support encryption, the connection will fail.
        /// </summary>
        Mandatory = 1,
        /// <summary>
        /// Enables and requires TDS 8.0, TLS encryption to the server. If the server doesn't support TDS 8.0, TLS encryption, the connection will fail.
        /// </summary>
        Strict = 2
    }

    /// <summary>
    /// Parse ODBC Driver 17 for SQL Server settings
    /// </summary>
    public static Dictionary<string, object> ParseSqlConnectionODBC_Settings_v17(string server,
        bool useIntegratedSecurity = false,
        bool encrypt = false,
        bool trustServerCertificate = false)
    {

        var settings = new Dictionary<string, object>
        {
            { "Driver", System.IO.Path.Combine(System.Environment.SystemDirectory, "msodbcsql17.dll") },       // NB: 32bit uses -> C:\WINDOWS\SysWOW64\msodbcsql17.dll
            { "Server", server }
        };

        // UseIntegratedSecurity
        if (useIntegratedSecurity)
        {
            settings.Add("Trusted_Connection", "Yes");
        }

        // Encrypt
        if (encrypt)
        {
            settings.Add("Encrypt", "Yes");
        }

        // TrustServerCertificate
        if (trustServerCertificate)
        {
            settings.Add("TrustServerCertificate", "Yes");
        }

        return settings;
    }

    /// <summary>
    /// Parse ODBC Driver 18 for SQL Server settings
    /// </summary>
    public static Dictionary<string, object> ParseSqlConnectionODBC_Settings_v18(string server,
        bool useIntegratedSecurity = false,
        SqlConnectionEncryptOption_v18 encryptOption = SqlConnectionEncryptOption_v18.Mandatory,
        bool trustServerCertificate = false,
        string? hostNameInCertificate = null)
    {
        var settings = new Dictionary<string, object>
        {
            { "Driver", System.IO.Path.Combine(System.Environment.SystemDirectory, "msodbcsql18.dll") },       // NB: 32bit uses -> C:\WINDOWS\SysWOW64\msodbcsql18.dll
            { "Server", server }
        };

        // UseIntegratedSecurity
        if (useIntegratedSecurity)
        {
            settings.Add("Trusted_Connection", "Yes");
        }

        // Encrypt
        switch (encryptOption)
        {
            case SqlConnectionEncryptOption_v18.Mandatory:
                break;
            case SqlConnectionEncryptOption_v18.Optional:
                settings.Add("Encrypt", "Optional");
                break;
            case SqlConnectionEncryptOption_v18.Strict:
                settings.Add("Encrypt", "Strict");
                break;
            default:
                throw new NotImplementedException();
        }

        // HostNameInCertificate
        if (hostNameInCertificate is not null)
        {
            settings.Add("HostNameInCertificate", hostNameInCertificate);
        }

        // TrustServerCertificate
        if (trustServerCertificate)
        {
            settings.Add("TrustServerCertificate", "Yes");
        }

        return settings;
    }

    /// <summary>
    /// Creates an completely custom ODBC User DSN in Windows Registry with the specified settings.
    /// </summary>
    public static void UserDSN_Create(string dsnName, Dictionary<string, object> dsnSettings)
    {
        var dsnKeyName = @$"SOFTWARE\ODBC\ODBC.INI\{dsnName}";

        // Delete previous key
        Microsoft.Win32.Registry.CurrentUser.DeleteSubKeyTree(dsnKeyName, throwOnMissingSubKey: false);

        // Create new key
        var dsnKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(dsnKeyName) ?? throw new InvalidOperationException($"Could not open registry key: {dsnKeyName}");

        // Configure
        foreach (var setting in dsnSettings)
        {
            dsnKey.SetValue(setting.Key, setting.Value);
        }
    }
}
