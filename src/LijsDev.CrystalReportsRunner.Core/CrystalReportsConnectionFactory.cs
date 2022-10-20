#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

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
        UseIntegratedSecurity = true
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
        UseIntegratedSecurity = false
    };
}
