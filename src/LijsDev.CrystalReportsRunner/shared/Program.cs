namespace LijsDev.CrystalReportsRunner;

using System;
using CommandLine;
using LijsDev.CrystalReportsRunner.Core;
using LijsDev.CrystalReportsRunner.Shell;
using static LijsDev.CrystalReportsRunner.Shell.Shell;

internal static class Program
{
    private static NLog.Logger? _logger;

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

#if CR_RUNTIME_13_0_20
    private const string CrystalReportsRuntimeVersion = "13.0.20";
#elif CR_RUNTIME_13_0_32
    private const string CrystalReportsRuntimeVersion = "13.0.32";
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
        var result = Parser.Default.ParseArguments<Options>(args);
        if (result.Tag == ParserResultType.Parsed)
        {
            var options = result.Value;
            NLogHelper.ConfigureNLog(options.LogPath, options.LogLevel);
        }

        _logger = NLog.LogManager.GetCurrentClassLogger();
        _logger?.Info("========================================================================================================");
        _logger?.Info($"LijsDev::CrystalReportsRunner::Program::Start::v{ApplicationVersion}");
        _logger?.Info("========================================================================================================");
        _logger?.Info($"LijsDev::CrystalReportsRunner::Program::Location::{ApplicationLocation}");
        _logger?.Info($"LijsDev::CrystalReportsRunner::Program::CrystalReportsRuntimeVersion::{CrystalReportsRuntimeVersion}");
        _logger?.Info($"LijsDev::CrystalReportsRunner::Program::CrystalReportsRuntimePlatform::{CrystalReportsRuntimePlatform}");
        _logger?.Info("========================================================================================================");

        try
        {
            // Enable Runtime LegacyV2 for Crystal Reports
            RuntimePolicyHelper.LegacyV2Runtime_Enable();

            var shell = new Shell.Shell(new ReportViewer(), new ReportExporter());
            shell.StartListening(args);
        }
        catch (Exception ex)
        {
            _logger?.Fatal(ex);
        }
        finally
        {
            _logger?.Info("========================================================================================================");
            _logger?.Info("LijsDev::CrystalReportsRunner::Program::End");
            _logger?.Info("========================================================================================================");
        }
    }
}
