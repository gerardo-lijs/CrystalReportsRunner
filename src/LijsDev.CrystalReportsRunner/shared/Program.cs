namespace LijsDev.CrystalReportsRunner;

using System;
using CommandLine;
using LijsDev.CrystalReportsRunner.Core;
using static LijsDev.CrystalReportsRunner.Shell.Shell;

internal static class Program
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public static string ApplicationVersion
    {
        get
        {
            var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(ApplicationLocation);
            return fileVersionInfo.ProductVersion;
        }
    }
    public static string ApplicationLocation => System.Reflection.Assembly.GetExecutingAssembly().Location;

    private static string CrystalReportsRuntimeVersion
    {
        get
        {
            var runnerPath = System.IO.Path.GetDirectoryName(ApplicationLocation);
            var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(System.IO.Path.Combine(runnerPath, "CrystalDecisions.CrystalReports.Engine.dll"));
            return fileVersionInfo.ProductVersion;
        }
    }

    private static string CrystalReportsRuntimePlatform => Environment.Is64BitProcess ? "x64" : "x86";

    [STAThread]
    private static void Main(string[] args)
    {
        try
        {
            var result = Parser.Default.ParseArguments<Options>(args);
            if (result.Tag == ParserResultType.Parsed)
            {
                var options = result.Value;
                NLogHelper.ConfigureNLog(options.LogDirectory, options.LogLevel);
            }
        }
        catch (Exception ex)
        {
            Logger.Fatal(ex);
        }

        Logger.Info("========================================================================================================");
        Logger.Info($"LijsDev::CrystalReportsRunner::Program::Start::v{ApplicationVersion}");
        Logger.Info("========================================================================================================");
        Logger.Info($"LijsDev::CrystalReportsRunner::Program::Location::{ApplicationLocation}");
        Logger.Info($"LijsDev::CrystalReportsRunner::Program::CrystalReportsRuntimeVersion::{CrystalReportsRuntimeVersion}");
        Logger.Info($"LijsDev::CrystalReportsRunner::Program::CrystalReportsRuntimePlatform::{CrystalReportsRuntimePlatform}");
        Logger.Info("========================================================================================================");

        try
        {
            // Enable Runtime LegacyV2 for Crystal Reports
            RuntimePolicyHelper.LegacyV2Runtime_Enable();

            var shell = new Shell.Shell(new ReportViewer(), new ReportExporter());
            shell.StartListening(args);
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
