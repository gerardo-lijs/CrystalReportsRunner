namespace LijsDev.CrystalReportsRunner;

using System;
using LijsDev.CrystalReportsRunner.Core;

internal static class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        // TODO: Add Logging

#if DEBUG
// Parse command line parameters
        var commandLineParameters = CommandLineParameters.Parse(args);

        if (commandLineParameters.DebugTest)
        {
            // NB: We can use this to test directly Reports without Named Pipes

            var reportViewer = new ReportViewer();

            var report = new Report("SampleReport.rpt", "Sample Report");
            report.Parameters.Add("ReportFrom", new DateTime(2022, 01, 01));
            report.Parameters.Add("UserName", "Muhammad");

            var viewerSettings = new ReportViewerSettings();
            var dbConnection = CrystalReportsConnectionFactory.CreateSqlConnection(".\\SQLEXPRESS", "CrystalReportsSample");

            var viewerForm = reportViewer.GetViewerForm(report, viewerSettings, dbConnection);

            viewerForm.ShowDialog();
        }
        else
        {
            var shell = new Shell.Shell(new ReportViewer());
            shell.StartListening(args);
        }
#else
        var shell = new Shell.Shell(new ReportViewer());
        shell.StartListening(args);
#endif
    }
}
