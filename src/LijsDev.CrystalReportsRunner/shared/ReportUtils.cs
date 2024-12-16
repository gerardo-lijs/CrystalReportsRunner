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

        // Cache logon properties
        var logonProperties = report.Connection is not null ? CreateLogonPropertiesFromConnection(report.Connection) : null;

        if (report.Connection is not null)
        {
            Logger.Trace("LijsDev::CrystalReportsRunner::ReportUtils::CreateReportDocument::Connection::Configuring");
            Logger.Trace($"LijsDev::CrystalReportsRunner::ReportUtils::CreateReportDocument::Connection::Server={report.Connection.Server}");
            Logger.Trace($"LijsDev::CrystalReportsRunner::ReportUtils::CreateReportDocument::Connection::Server={report.Connection.Database}");

            for (var i = 0; i < document.DataSourceConnections.Count; i++)
            {
                ConfigureDataSourceConnection(document.DataSourceConnections[i], report.Connection, logonProperties);

                // NB: We need to set data source configuration twice, otherwise it does not work. Very strange Crystal Reports behaviour. Could be improved with the right code/order to set connections.
                ConfigureDataSourceConnection(document.DataSourceConnections[i], report.Connection, logonProperties);
            }

            foreach (ReportDocument subReport in document.Subreports)
            {
                for (var i = 0; i < subReport.DataSourceConnections.Count; i++)
                {
                    ConfigureDataSourceConnection(subReport.DataSourceConnections[i], report.Connection, logonProperties);

                    // NB: We need to set data source configuration twice, otherwise it does not work. Very strange Crystal Reports behaviour. Could be improved with the right code/order to set connections.
                    ConfigureDataSourceConnection(subReport.DataSourceConnections[i], report.Connection, logonProperties);
                }
            }
        }

        // Main Report
        foreach (Table crTable in document.Database.Tables)
        {
            ConfigureTableConnection(crTable, report.Connection, logonProperties);
            ConfigureTableDataSource(crTable, report.DataSets);
        }
        // Sub Reports
        foreach (ReportDocument crSubReport in document.Subreports)
        {
            foreach (Table crTable in crSubReport.Database.Tables)
            {
                ConfigureTableConnection(crTable, report.Connection, logonProperties);
                ConfigureTableDataSource(crTable, report.DataSets);
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

    private static NameValuePairs2 CreateLogonPropertiesFromConnection(CrystalReportsConnection crystalReportsConnection)
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

        return logonProperties;
    }

    private static void ConfigureDataSourceConnection(IConnectionInfo connection, CrystalReportsConnection? crystalReportsConnection, NameValuePairs2? logonProperties)
    {
        if (crystalReportsConnection is null) return;

        // Apply logon properties
        if (logonProperties is not null)
        {
            connection.SetLogonProperties(logonProperties);
        }

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

    private static void ConfigureTableConnection(Table crTable, CrystalReportsConnection? crystalReportsConnection, NameValuePairs2? logonProperties)
    {
        if (crystalReportsConnection is null) return;

        crTable.LogOnInfo.ConnectionInfo.ServerName = crystalReportsConnection.Server;
        crTable.LogOnInfo.ConnectionInfo.DatabaseName = crystalReportsConnection.Database;
        crTable.LogOnInfo.ConnectionInfo.UserID = crystalReportsConnection.Username;
        crTable.LogOnInfo.ConnectionInfo.Password = crystalReportsConnection.Password;
        crTable.LogOnInfo.ConnectionInfo.IntegratedSecurity = crystalReportsConnection.UseIntegratedSecurity;

        if (logonProperties is not null)
        {
            crTable.LogOnInfo.ConnectionInfo.LogonProperties = logonProperties;
        }

        crTable.ApplyLogOnInfo(crTable.LogOnInfo);
    }

    private static void ConfigureTableDataSource(Table crTable, List<DataSet> datasets)
    {
        foreach (var item in datasets)
        {
            if (item.Tables.Contains(crTable.Name))
            {
                crTable.SetDataSource(item.Tables[crTable.Name]);
                return;
            }
        }
    }
}
