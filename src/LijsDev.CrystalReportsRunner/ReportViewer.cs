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
    public Form GetViewerForm(Report report, ReportViewerSettings viewerSettings)
    {
        var document = CreateReportDocument(report);
        return new ViewerForm(document, viewerSettings)
        {
            Text = report.Title
        };
    }

    private static ReportDocument CreateReportDocument(Report report)
    {
        var document = new ReportDocument();
        document.Load(report.Filename);

        ConnectionInfo? crConnection;

        if (report.Connection is not null)
        {
            var logonProperties = new NameValuePairs2()
            {
                new NameValuePair2("Data Source", report.Connection.Server),
                new NameValuePair2("Initial Catalog", report.Connection.Database),
                new NameValuePair2("Integrated Security", report.Connection.UseIntegratedSecurity ? "False" : "True"),
            };
            if (report.Connection.LogonProperties is not null)
            {
                foreach (var property in report.Connection.LogonProperties)
                {
                    logonProperties.Add(new NameValuePair2(property.Key, property.Value));
                }
            }

            foreach (IConnectionInfo connection in document.DataSourceConnections)
            {
                connection.SetLogonProperties(logonProperties);
                if (report.Connection.UseIntegratedSecurity)
                {
                    connection.SetConnection(
                        report.Connection.Server, report.Connection.Database, useIntegratedSecurity: true);
                }
                else
                {
                    connection.SetConnection(
                        report.Connection.Server,
                        report.Connection.Database,
                        report.Connection.Username,
                        report.Connection.Password);
                }
            }

            // Set table connection
            crConnection = new ConnectionInfo
            {
                ServerName = report.Connection.Server,
                DatabaseName = report.Connection.Database
            };

            if (report.Connection.UseIntegratedSecurity)
            {
                crConnection.IntegratedSecurity = true;
            }
            else
            {
                crConnection.UserID = report.Connection.Username;
                crConnection.Password = report.Connection.Password;
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
