namespace LijsDev.CrystalReportsRunner;

using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

using LijsDev.CrystalReportsRunner.Core;
using LijsDev.CrystalReportsRunner.Shell;

internal class ReportViewer : IReportViewer
{
    public Form GetViewerForm(Report report, ReportViewerSettings viewerSettings, CrystalReportsConnection? dbConnection)
    {
        var document = CreateReportDocument(report, dbConnection);
        return new ViewerForm(document, viewerSettings)
        {
            Text = report.Title
        };
    }

    private static ReportDocument CreateReportDocument(Report report, CrystalReportsConnection? dbConnection)
    {
        var document = new ReportDocument();
        document.Load(report.Filename);

        ConnectionInfo? crConnection;

        if (dbConnection is not null)
        {
            var logonProperties = new NameValuePairs2()
            {
                new NameValuePair2("Data Source", dbConnection.Server),
                new NameValuePair2("Initial Catalog", dbConnection.Database),
                new NameValuePair2("Integrated Security", dbConnection.UseIntegratedSecurity ? "False" : "True"),
            };
            if (dbConnection.LogonProperties is not null)
            {
                foreach (var property in dbConnection.LogonProperties)
                {
                    logonProperties.Add(new NameValuePair2(property.Key, property.Value));
                }
            }

            foreach (IConnectionInfo connection in document.DataSourceConnections)
            {
                connection.SetLogonProperties(logonProperties);
                if (dbConnection.UseIntegratedSecurity)
                {
                    connection.SetConnection(
                        dbConnection.Server, dbConnection.Database, useIntegratedSecurity: true);
                }
                else
                {
                    connection.SetConnection(
                        dbConnection.Server,
                        dbConnection.Database,
                        dbConnection.Username,
                        dbConnection.Password);
                }
            }

            // Set table connection
            crConnection = new ConnectionInfo
            {
                ServerName = dbConnection.Server,
                DatabaseName = dbConnection.Database
            };

            if (dbConnection.UseIntegratedSecurity)
            {
                crConnection.IntegratedSecurity = true;
            }
            else
            {
                crConnection.UserID = dbConnection.Username;
                crConnection.Password = dbConnection.Password;
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

        // NB: SummaryInfo.ReportTitle is used as initial value in save dialog when exporting a report.
        document.SummaryInfo.ReportTitle = report.ExportFilename ?? report.Title;

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
        if (table is not null)
        {
            crTable.SetDataSource(table);
        }
        else if (crConnection is not null)
        {
            var tableLogonInfo = crTable.LogOnInfo;
            tableLogonInfo.ConnectionInfo = crConnection;
            crTable.ApplyLogOnInfo(tableLogonInfo);
        }
    }
}
