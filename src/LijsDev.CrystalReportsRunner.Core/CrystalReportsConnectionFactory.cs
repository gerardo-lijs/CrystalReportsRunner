namespace LijsDev.CrystalReportsRunner.Core;

/// <summary>
/// Crystal Reports Connection Factory
/// </summary>
public static class CrystalReportsConnectionFactory
{
    /// <summary>
    /// Microsoft SQL Server encrypt options
    /// </summary>
    public enum SqlConnection_EncryptOption
    {
        /// <summary>
        /// Encryption will not be enforced and SQL Server configuration will determine if encryption is enabled or not.
        /// </summary>
        Optional,
        /// <summary>
        /// Encryption will be enforced by client.
        /// </summary>
        Mandatory,
        /// <summary>
        /// Encryption will be enforced by client. SQL Server 2022 option.
        /// </summary>
        Strict
    }

    /// <summary>
    /// Create a Microsoft OLE DB SQL Server connection using Integrated Security
    /// </summary>
    public static CrystalReportsConnection CreateSqlConnection(string server, string database, bool encrypt = false, bool trustServerCertificate = false) =>
        CreateSqlConnection(server, database, useIntegratedSecurity: true, username: string.Empty, password: string.Empty, encrypt, trustServerCertificate);

    /// <summary>
    /// Create a Microsoft OLE DB SQL Server connection using SQL username and password
    /// </summary>
    public static CrystalReportsConnection CreateSqlConnection(string server, string database, string username, string password, bool encrypt = false, bool trustServerCertificate = false) =>
        CreateSqlConnection(server, database, useIntegratedSecurity: false, username, password, encrypt, trustServerCertificate);

    /// <summary>
    /// Create a Microsoft OLE DB SQL Server connection
    /// </summary>
    public static CrystalReportsConnection CreateSqlConnection(string server, string database, bool useIntegratedSecurity, string username = "", string password = "", bool encrypt = false, bool trustServerCertificate = false)
    {
        var conn = new CrystalReportsConnection()
        {
            Server = server,
            Database = database,
            Username = username,
            Password = password,
            UseIntegratedSecurity = useIntegratedSecurity,
            LogonProperties = new()
            {
                { "Provider", "MSOLEDBSQL" },
                { "Auto Translate", "False" },
                { "Connect Timeout", "15" },
                { "General Timeout", "0" },
                { "Locale Identifier", "1033" }, // English - United States
                { "Tag with column collation when possible", "False" },
                { "Use Encryption for Data", encrypt ? "True" : "False" },
            }
        };

        if (trustServerCertificate)
        {
            conn.LogonProperties.Add("Trust Server Certificate", "True");
        }

        return conn;
    }

    /// <summary>
    /// Creates a Microsoft OLE DB v19 SQL Server connection using Integrated Security
    /// </summary>
    public static CrystalReportsConnection CreateConnection_MSOLEDBSQL19(string server, string database, SqlConnection_EncryptOption encrypt = SqlConnection_EncryptOption.Optional, bool trustServerCertificate = false, string hostNameInCertificate = "") =>
        CreateConnection_MSOLEDBSQL19(server, database, useIntegratedSecurity: true, username: string.Empty, password: string.Empty, encrypt, trustServerCertificate, hostNameInCertificate);

    /// <summary>
    /// Creates a Microsoft OLE DB v19 SQL Server connection using SQL username and password
    /// </summary>
    public static CrystalReportsConnection CreateConnection_MSOLEDBSQL19(string server, string database, string username, string password, SqlConnection_EncryptOption encrypt = SqlConnection_EncryptOption.Optional, bool trustServerCertificate = false, string hostNameInCertificate = "") =>
        CreateConnection_MSOLEDBSQL19(server, database, useIntegratedSecurity: false, username, password, encrypt, trustServerCertificate, hostNameInCertificate);

    /// <summary>
    /// Creates a Microsoft OLE DB v19 SQL Server connection
    /// </summary>
    public static CrystalReportsConnection CreateConnection_MSOLEDBSQL19(string server, string database, bool useIntegratedSecurity = true, string username = "", string password = "", SqlConnection_EncryptOption encrypt = SqlConnection_EncryptOption.Optional, bool trustServerCertificate = false, string hostNameInCertificate = "")
    {
        var conn = new CrystalReportsConnection()
        {
            Server = server,
            Database = database,
            Username = username,
            Password = password,
            UseIntegratedSecurity = useIntegratedSecurity,
            LogonProperties = new()
            {
                { "Provider", "MSOLEDBSQL19" },
                { "Auto Translate", "False" },
                { "Connect Timeout", "15" },
                { "General Timeout", "0" },
                { "Locale Identifier", "1033" }, // English - United States
                { "Tag with column collation when possible", "False" },
                { "Use Encryption for Data", $"{encrypt}" },
            }
        };

        if (!string.IsNullOrEmpty(hostNameInCertificate))
        {
            conn.LogonProperties.Add("Host Name In Certificate", hostNameInCertificate);
        }
        if (trustServerCertificate)
        {
            conn.LogonProperties.Add("Trust Server Certificate", "True");
        }

        return conn;
    }

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

    private static Dictionary<string, string> ODBCSqlLogonProperties => new()
    {
        { "Connect Timeout", "15" },
        { "General Timeout", "0" },
        { "Locale Identifier", "1033" }, // English - United States
        { "Provider", "MSDASQL" },
        { "Use DSN Default Properties", "False" },
    };
}
