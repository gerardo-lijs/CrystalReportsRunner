namespace LijsDev.CrystalReportsRunner;

using System;
using LijsDev.CrystalReportsRunner.Core;
using NLog;

internal static class Program
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public static string ApplicationVersion
    {
        get
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.ProductVersion;
        }
    }
    public static string ApplicationLocation => System.Reflection.Assembly.GetExecutingAssembly().Location;

    // TODO: Get RuntimeVersion and Platform from csproj directly
    private const string CrystalReportsRuntimeVersion = "13.0.20";
    private const string CrystalReportsRuntimePlatform = "x86";

    [STAThread]
    private static void Main(string[] args)
    {
        Logger.Info("========================================================================================================");
        Logger.Info($"LijsDev::CrystalReportsRunner::Program::Start::v{ApplicationVersion}");
        Logger.Info("========================================================================================================");
        Logger.Info($"LijsDev::CrystalReportsRunner::Program::Location::{ApplicationLocation}");
        Logger.Info($"LijsDev::CrystalReportsRunner::Program::CrystalReportsRuntimeVersion::{CrystalReportsRuntimeVersion}");
        Logger.Info($"LijsDev::CrystalReportsRunner::Program::CrystalReportsRuntimePlatform::{CrystalReportsRuntimePlatform}");
        Logger.Info("========================================================================================================");

        try
        {
#if DEBUG
            Logger.Trace("LijsDev::CrystalReportsRunner::Program::Starting in DEBUG mode");

            // Parse command line parameters
            var commandLineParameters = CommandLineParameters.Parse(args);

            if (commandLineParameters.DebugTest)
            {
                // NB: We can use this to test directly Reports without Named Pipes
                var report = new Report("SampleReport.rpt", "Sample Report", "sample_export")
                {
                    Connection = CrystalReportsConnectionFactory.CreateSqlConnection(".\\SQLEXPRESS", "CrystalReportsSample")
                };
                report.Parameters.Add("ReportFrom", new DateTime(2022, 01, 01));
                report.Parameters.Add("UserName", "Muhammad");

                // Test 1 - Show Viewer
                var reportViewer = new ReportViewer();
                var viewerSettings = new ReportViewerSettings
                {
                    WindowInitialState = ReportViewerWindowState.Maximized,
                    WindowInitialPosition = ReportViewerWindowStartPosition.CenterScreen
                };

                using var viewerForm = reportViewer.GetViewerForm(report, viewerSettings);
                viewerForm.ShowDialog();

                // Test 2 - Export Report
                //var reportExporter = new ReportExporter();
                //reportExporter.Export(report, ReportExportFormats.PDF, "sample_report.pdf");
            }
            else
            {
                var shell = new Shell.Shell(new ReportViewer(), new ReportExporter());
                shell.StartListening(args);
            }
#else
        var shell = new Shell.Shell(new ReportViewer(), new ReportExporter());
        shell.StartListening(args);
#endif
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex);
        }
        finally
        {
            Logger.Info("========================================================================================================");
            Logger.Info("LijsDev::CrystalReportsRunner::Program::End");
            Logger.Info("========================================================================================================");
        }
    }
}
