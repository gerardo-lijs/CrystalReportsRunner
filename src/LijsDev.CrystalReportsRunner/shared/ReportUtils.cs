namespace LijsDev.CrystalReportsRunner;

using Core;
using CrystalDecisions.CrystalReports.Engine;
using NLog;
using Shell;

internal static class ReportUtils
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static CustomReportDocument CreateReportDocument(Report report)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::ReportUtils::CreateReportDocument::Filename={report.Filename}");

        var doc = new CustomReportDocument();
        try
        {
            var dbChanged = false;

            doc.Load(report.Filename);

            doc.CallbackGuid = report.Guid;

            //Check Report Datenbankverbindung geändert?
            if (report.Connection is not null)
            {
                if (doc.DataSourceConnections[0].ServerName != report.Connection.Server
                    || doc.DataSourceConnections[0].DatabaseName != report.Connection.Database
                    || doc.DataSourceConnections[0].IntegratedSecurity != report.Connection.UseIntegratedSecurity)
                {
                    doc.DataSourceConnections.Clear();
                    doc.DataSourceConnections[0]
                        .SetConnection(report.Connection.Server, report.Connection.Database, report.Connection.UseIntegratedSecurity);
                    dbChanged = true;
                }

                //Hat die Datenbankverbindung in den Subreports geändert?
                foreach (ReportDocument subReport in doc.Subreports)
                {
                    subReport.DataSourceConnections.Clear();
                    if (subReport.DataSourceConnections[0].ServerName != report.Connection.Server
                        || subReport.DataSourceConnections[0].DatabaseName != report.Connection.Database
                        || subReport.DataSourceConnections[0].IntegratedSecurity != report.Connection.UseIntegratedSecurity)
                    {
                        subReport.DataSourceConnections[0]
                            .SetConnection(report.Connection.Server, report.Connection.Database, report.Connection.UseIntegratedSecurity);
                        dbChanged = true;
                    }
                }
            }

            //Benutzername und Passwort, falls SQL-Authentifizierung
            if (!doc.DataSourceConnections[0].IntegratedSecurity)
            {
                doc.DataSourceConnections[0].SetLogon(report.Connection?.Username, report.Connection?.Password);
            }

            //Changed?
            if (dbChanged)
            {
                if (report.Connection?.UseIntegratedSecurity != true)
                {
                    try
                    {
                        doc.VerifyDatabase();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex, "Could not verify the database connection.");
                    }
                }

                doc.SaveAs(report.Filename, false);
            }

            //Parameter setzen
            SetReportParameters(doc, report.WhereStatement, report.Parameters);
        }
        catch (Exception ex)
        {
            var logon = "";
            if (doc != null)
            {
                if (doc.IsLoaded)
                {
                    logon = doc.DataSourceConnections[0].ServerName + "\r\n" +
                            doc.DataSourceConnections[0].DatabaseName + "\r\n" +
                            doc.DataSourceConnections[0].Type;
                }
            }

            var mess = ex.Message;
            if (ex.InnerException != null)
            {
                mess = ex.InnerException.Message;
            }

            throw new Exception("Failed to open report:" + "'" + report.Filename + "'\r\n" + logon + "\r\n Exception:" + mess);
        }

        return doc;
    }

    private static void SetReportParameters(CustomReportDocument reportDocument, string whereStatement, Dictionary<string, object> parameters)
    {
        var reportParameters = parameters;

        //StandardParameter vordefinieren
        foreach (var parameter in reportParameters)
        {
            reportDocument.SetParameterValueIfExists(parameter.Key, parameter.Value);
        }

        //WHERE Setzen
        if (whereStatement.Length > 0)
        {
            if (whereStatement.Contains("{"))
            {
                if (reportDocument.RecordSelectionFormula.Length > 0)
                {
                    reportDocument.RecordSelectionFormula = "(" + reportDocument.RecordSelectionFormula +
                                                            ")  AND  (" + whereStatement + ")";
                }
                else
                {
                    reportDocument.RecordSelectionFormula = whereStatement;
                }
            }
            else
            {
                var splitParameter = whereStatement.Split('=');
                var whereParameter = reportDocument.ParameterFields.Find(splitParameter[0], "");
                if (whereParameter != null)
                {
                    reportDocument.SetParameterValue(splitParameter[0], splitParameter[1]);
                }
            }
        }
    }
}
