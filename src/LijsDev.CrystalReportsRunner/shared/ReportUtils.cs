namespace LijsDev.CrystalReportsRunner;

using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

using System.Collections.Generic;
using System.Data;

using LijsDev.CrystalReportsRunner.Core;

internal static class ReportUtils
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public static ReportDocument CreateReportDocument(Report report)
    {
        Logger.Trace("LijsDev::CrystalReportsRunner::ReportUtils::CreateReportDocument::Start");
        Logger.Trace($"LijsDev::CrystalReportsRunner::ReportUtils::CreateReportDocument::Filename={report.Filename}");

        var document = new ReportDocument();
        document.Load(report.Filename);

        Logger.Trace("LijsDev::CrystalReportsRunner::ReportUtils::CreateReportDocument::ReportDocument::Loaded");

        if (report.Connection is not null)
        {
            Logger.Trace("LijsDev::CrystalReportsRunner::ReportUtils::CreateReportDocument::Connection::Configuring");
            Logger.Trace($"LijsDev::CrystalReportsRunner::ReportUtils::CreateReportDocument::Connection::Server={report.Connection.Server}");
            Logger.Trace($"LijsDev::CrystalReportsRunner::ReportUtils::CreateReportDocument::Connection::Server={report.Connection.Database}");

            document.DataSourceConnections.Clear();
            SetDataSourceConnection(report.Connection, document.DataSourceConnections[0]);

            // NB: We need to set data source configuration twice, otherwise it does not work. Very strange Crystal Reports behaviour. Could be improved with the right code/order to set connections.
            SetDataSourceConnection(report.Connection, document.DataSourceConnections[0]);

            foreach (ReportDocument subReport in document.Subreports)
            {
                if (subReport.DataSourceConnections.Count > 0)
                {
                    subReport.DataSourceConnections.Clear();
                    SetDataSourceConnection(report.Connection, subReport.DataSourceConnections[0]);

                    // NB: We need to set data source configuration twice, otherwise it does not work. Very strange Crystal Reports behaviour. Could be improved with the right code/order to set connections.
                    SetDataSourceConnection(report.Connection, subReport.DataSourceConnections[0]);
                }
            }
        }

        var crConnection = report.Connection is not null ? document.DataSourceConnections[0] as ConnectionInfo : null;
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
                Logger.Trace($"LijsDev::CrystalReportsRunner::ReportUtils::CreateReportDocument::SetParameter={parameter.ParameterFieldName} | Value={value}");
                parameter.CurrentValues.Clear();
                if (value.GetType() != typeof(string) && value is System.Collections.IEnumerable valueEnumerable)
                {
                    foreach (var valueItem in valueEnumerable)
                    {
                        parameter.CurrentValues.AddValue(valueItem);
                    }
                }
                else
                {
                    parameter.CurrentValues.AddValue(value);
                }
            }
        }

        // NB: SummaryInfo.ReportTitle is used as initial value in save dialog when exporting a report.
        document.SummaryInfo.ReportTitle = report.ExportFilename ?? report.Title;

        Logger.Trace("LijsDev::CrystalReportsRunner::ReportUtils::CreateReportDocument::End");
        return document;
    }

    private static void SetDataSourceConnection(CrystalReportsConnection crystalReportsConnection, IConnectionInfo connection)
    {
        var logonProperties = new NameValuePairs2()
        {
            new NameValuePair2("Data Source", crystalReportsConnection.Server),
            new NameValuePair2("Initial Catalog", crystalReportsConnection.Database),
            new NameValuePair2("Integrated Security", crystalReportsConnection.UseIntegratedSecurity),
        };
        if (crystalReportsConnection.LogonProperties is not null)
        {
            foreach (var property in crystalReportsConnection.LogonProperties)
            {
                logonProperties.Add(new NameValuePair2(property.Key, property.Value));
            }
        }

        // Apply logon properties
        connection.SetLogonProperties(logonProperties);

        // Apply connection
        connection.IntegratedSecurity = crystalReportsConnection.UseIntegratedSecurity;
        if (crystalReportsConnection.UseIntegratedSecurity)
        {
            connection.SetConnection(
                crystalReportsConnection.Server, crystalReportsConnection.Database, useIntegratedSecurity: true);
        }
        else
        {
            connection.SetLogon(crystalReportsConnection.Username, crystalReportsConnection.Password);

            connection.SetConnection(
                crystalReportsConnection.Server,
                crystalReportsConnection.Database,
                crystalReportsConnection.Username,
                crystalReportsConnection.Password);
        }
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
