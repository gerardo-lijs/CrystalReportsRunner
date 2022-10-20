namespace LijsDev.CrystalReportsRunner.Core;

public class DbConnection
{
    public DbConnection()
    {

    }

    /// <summary>
    /// Create a DbConnection setting from a connection string.
    /// </summary>
    /// <param name="connectionString"></param>
    public DbConnection(string connectionString)
    {
        ConnectionString = connectionString;
    }

    /// <summary>
    /// Create a DbConnection that uses Integrated Security.
    /// </summary>
    /// <param name="server"></param>
    /// <param name="database"></param>
    public DbConnection(string server, string database)
    {
        Server = server;
        Database = database;
        UseIntegratedSecurity = true;
    }

    /// <summary>
    /// Create a DbConnection that uses SQL Server authentication.
    /// </summary>
    /// <param name="server"></param>
    /// <param name="database"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public DbConnection(string server, string database, string username, string password)
    {
        Server = server;
        Database = database;
        Username = username;
        Password = password;
        UseIntegratedSecurity = false;
    }

    public string? ConnectionString { get; set; }
    public string? Server { get; set; }
    public string? Database { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public bool UseIntegratedSecurity { get; set; }
}

