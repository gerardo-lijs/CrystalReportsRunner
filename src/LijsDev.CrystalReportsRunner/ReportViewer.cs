namespace LijsDev.CrystalReportsRunner;

using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

using LijsDev.CrystalReportsRunner.Abstractions;
using LijsDev.CrystalReportsRunner.Core;

internal class ReportViewer : IReportViewer
{
    public Form GetViewerForm(Report report, ReportViewerSettings viewerSettings, DbConnection? dbConnection)
    {
        var document = CreateReportDocument(report, dbConnection);
        return new ViewerForm(document, viewerSettings);
    }

    private static ReportDocument CreateReportDocument(Report report, DbConnection? dbConnection)
    {
        var document = new ReportDocument();
        document.Load(report.Path);

        ConnectionInfo? crConnection;

        if (dbConnection != null)
        {
            var connectionSettings = dbConnection.GetConnectionSettings();

            var logonProperties = new NameValuePairs2
            {
                new NameValuePair2("Auto Translate", "-1"),
                new NameValuePair2("Connect Timeout", "15"),
                new NameValuePair2("Data Source", connectionSettings.DataSource),
                new NameValuePair2("General Timeout", "0"),
                new NameValuePair2("Initial Catalog", connectionSettings.InitialCatalog),
                new NameValuePair2("Integrated Security", connectionSettings.IntegratedSecurity ? "False" : "True"),
                new NameValuePair2("Locale Identifier", "1033"),
                new NameValuePair2("OLE DB Services", "-5"),
                new NameValuePair2("Provider", "MSOLEDBSQL"),
                new NameValuePair2("Tag with column collation when possible", "0"),
                new NameValuePair2("Use DSN Default Properties", "False"),
                new NameValuePair2("Use Encryption for Data", "0")
            };

            foreach (IConnectionInfo connection in document.DataSourceConnections)
            {
                connection.SetLogonProperties(logonProperties);
                if (connectionSettings.IntegratedSecurity)
                {
                    connection.SetConnection(
                        connectionSettings.DataSource, connectionSettings.InitialCatalog, useIntegratedSecurity: true);
                }
                else
                {
                    connection.SetConnection(
                        connectionSettings.DataSource,
                        connectionSettings.InitialCatalog,
                        connectionSettings.UserID,
                        connectionSettings.Password);
                }
            }

            // Set table connection
            crConnection = new ConnectionInfo
            {
                ServerName = connectionSettings.DataSource,
                DatabaseName = connectionSettings.InitialCatalog
            };

            if (connectionSettings.IntegratedSecurity)
            {
                crConnection.IntegratedSecurity = true;
            }
            else
            {
                crConnection.UserID = connectionSettings.UserID;
                crConnection.Password = connectionSettings.Password;
            }
        }
        else
        {
            crConnection = null;
        }

        // Main Report
        foreach (Table crTable in document.Database.Tables)
        {
            ConfigureTableDataSource(crTable, report.DataSets, crConnection);
        }
        // Sub Reports
        foreach (ReportDocument crSubReport in document.Subreports)
        {
            foreach (Table crTable in crSubReport.Database.Tables)
            {
                ConfigureTableDataSource(crTable, report.DataSets, crConnection);
            }
        }

        // Set parameters
        foreach (ParameterField parameter in document.ParameterFields)
        {
            if (report.Parameters.TryGetValue(parameter.ParameterFieldName, out var value))
            {
                document.SetParameterValue(parameter.ParameterFieldName, value);
            }
        }

        if (report.Title is not null)
        {
            document.SummaryInfo.ReportTitle = report.Title;
        }

        return document;
    }

    private static void ConfigureTableDataSource(Table crTable, List<DataSet> datasets, ConnectionInfo? crConnection)
    {
        DataTable? table = null;
        foreach (var item in datasets)
        {
            if (item.Tables.Contains(crTable.Name))
            {
                table = item.Tables[crTable.Name];
                break;
            }
        }

        // If no dataset is supplied, configure its connection string.
        if (table != null)
        {
            crTable.SetDataSource(table);
        }
        else if (crConnection != null)
        {
            var tableLogonInfo = crTable.LogOnInfo;
            tableLogonInfo.ConnectionInfo = crConnection;
            crTable.ApplyLogOnInfo(tableLogonInfo);
        }
    }
}
