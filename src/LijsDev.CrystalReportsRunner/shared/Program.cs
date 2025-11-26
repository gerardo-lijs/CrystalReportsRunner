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
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.ProductVersion;
        }
    }
    public static string ApplicationLocation => System.Reflection.Assembly.GetExecutingAssembly().Location;

#if CR_RUNTIME_13_0_16
    private const string CrystalReportsRuntimeVersion = "13.0.16";
#elif CR_RUNTIME_13_0_20
    private const string CrystalReportsRuntimeVersion = "13.0.20";
#elif CR_RUNTIME_13_0_32
    private const string CrystalReportsRuntimeVersion = "13.0.32";
#elif CR_RUNTIME_13_0_33
    private const string CrystalReportsRuntimeVersion = "13.0.33";
#elif CR_RUNTIME_13_0_34
    private const string CrystalReportsRuntimeVersion = "13.0.34";
#elif CR_RUNTIME_13_0_38
    private const string CrystalReportsRuntimeVersion = "13.0.38";
#else
    private const string CrystalReportsRuntimeVersion = "Unknown";
#endif

#if CR_RUNTIME_x86
    private const string CrystalReportsRuntimePlatform = "x86";
#elif CR_RUNTIME_x64
    private const string CrystalReportsRuntimePlatform = "x64";
#else
    private const string CrystalReportsRuntimePlatform = "Unknown";
#endif

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
