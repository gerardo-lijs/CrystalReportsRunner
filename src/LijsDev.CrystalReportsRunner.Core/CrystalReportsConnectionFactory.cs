namespace LijsDev.CrystalReportsRunner.Core;

/// <summary>
/// Crystal Reports Connection Factory
/// </summary>
public static class CrystalReportsConnectionFactory
{
    /// <summary>
    /// Create a SQL Server connection using Integrated Security
    /// </summary>
    public static CrystalReportsConnection CreateSqlConnection(string server, string database) => new()
    {
        Server = server,
        Database = database,
        UseIntegratedSecurity = true,
        LogonProperties = SqlLogonProperties
    };

    /// <summary>
    /// Create a SQL Server connection using SQL username and password
    /// </summary>
    public static CrystalReportsConnection CreateSqlConnection(string server, string database, string username, string password) => new()
    {
        Server = server,
        Database = database,
        Username = username,
        Password = password,
        UseIntegratedSecurity = false,
        LogonProperties = SqlLogonProperties
    };

    /// <summary>
    /// Create a ODBC Driver 18 for SQL Server connection using Integrated Security
    /// </summary>
    public static CrystalReportsConnection CreateODBCSqlConnection(string dsnName, string database) => new()
    {
        Server = dsnName,
        Database = database,
        UseIntegratedSecurity = true,
        LogonProperties = ODBCSqlLogonProperties
    };

    /// <summary>
    /// Create a ODBC Driver 18 for SQL Server connection using SQL username and password
    /// </summary>
    public static CrystalReportsConnection CreateODBCSqlConnection(string dsnName, string database, string username, string password) => new()
    {
        Server = dsnName,
        Database = database,
        Username = username,
        Password = password,
        UseIntegratedSecurity = false,
        LogonProperties = ODBCSqlLogonProperties
    };

    private static Dictionary<string, string> SqlLogonProperties => new()
    {
        { "Auto Translate", "-1" },
        { "Connect Timeout", "15" },
        { "General Timeout", "0" },
        { "Locale Identifier", "1033" },
        { "OLE DB Services", "-5" },
        { "Provider", "MSOLEDBSQL" },
        { "Tag with column collation when possible", "0" },
        { "Use DSN Default Properties", "False" },
        { "Use Encryption for Data", "0" },
    };

    private static Dictionary<string, string> ODBCSqlLogonProperties => new()
    {
        { "Connect Timeout", "15" },
        { "General Timeout", "0" },
        { "Locale Identifier", "1033" },
        { "OLE DB Services", "-5" },
        { "Provider", "MSDASQL" },
        { "Use DSN Default Properties", "False" },
    };
}
