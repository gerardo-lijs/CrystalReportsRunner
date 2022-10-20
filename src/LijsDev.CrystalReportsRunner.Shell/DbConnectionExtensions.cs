namespace LijsDev.CrystalReportsRunner.Shell;

using System.Data.SqlClient;

using LijsDev.CrystalReportsRunner.Core;

public static class DbConnectionExtensions
{
    public static SqlConnectionStringBuilder GetConnectionSettings(this DbConnection dbConnection)
    {
        if (dbConnection.ConnectionString is not null)
        {
            return new SqlConnectionStringBuilder(dbConnection.ConnectionString);
        }

        return new SqlConnectionStringBuilder
        {
            DataSource = dbConnection.Server,
            UserID = dbConnection.Username,
            Password = dbConnection.Password,
            InitialCatalog = dbConnection.Database,
            IntegratedSecurity = dbConnection.UseIntegratedSecurity,
        };
    }
}
