namespace LijsDev.CrystalReportsRunner;

using System.Diagnostics;
using System.Reflection;
using System.Windows;
using CommandLine;
using Core;
using NLog;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static string ApplicationVersion
    {
        get
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return fileVersionInfo.ProductVersion;
        }
    }

    public static string ApplicationLocation => Assembly.GetExecutingAssembly().Location;

#if CR_RUNTIME_13_0_20
    private const string CrystalReportsRuntimeVersion = "13.0.20";
#elif CR_RUNTIME_13_0_32
    private const string CrystalReportsRuntimeVersion = "13.0.32";
#elif CR_RUNTIME_13_0_33
    private const string CrystalReportsRuntimeVersion = "13.0.33";
#elif CR_RUNTIME_13_0_34
    private const string CrystalReportsRuntimeVersion = "13.0.34";
#elif CR_RUNTIME_13_0_36
    private const string CrystalReportsRuntimeVersion = "13.0.36";
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

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        DispatcherUnhandledException += (s, exArgs) =>
        {
            Logger.Fatal(exArgs.Exception, "Unhandled exception in WPF UI thread");
            MessageBox.Show(
                exArgs.Exception.ToString(),
                "Unhandled Exception",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Current.Shutdown(-1);
        };

        // Exceptions thrown here will be caught in DispatcherUnhandledExceptions above
        var result = Parser.Default.ParseArguments<Shell.Shell.Options>(e.Args);
        if (result.Tag == ParserResultType.Parsed)
        {
            var options = result.Value;
            NLogHelper.ConfigureNLog(options.LogDirectory, options.LogLevel);
        }

        Logger.Info("========================================================================================================");
        Logger.Info($"LijsDev::CrystalReportsRunner::Program::Start::v{ApplicationVersion}");
        Logger.Info("========================================================================================================");
        Logger.Info($"LijsDev::CrystalReportsRunner::Program::Location::{ApplicationLocation}");
        Logger.Info($"LijsDev::CrystalReportsRunner::Program::CrystalReportsRuntimeVersion::{CrystalReportsRuntimeVersion}");
        Logger.Info($"LijsDev::CrystalReportsRunner::Program::CrystalReportsRuntimePlatform::{CrystalReportsRuntimePlatform}");
        Logger.Info("========================================================================================================");

        RuntimePolicyHelper.LegacyV2Runtime_Enable();

        var shell = new Shell.Shell(new ReportViewer(), new ReportExporter(), Dispatcher);
        shell.StartListening(e.Args).ContinueWith((task) =>
        {
            // Exceptions thrown inside Shell.StartListening will be handled here
            if (task.Status != TaskStatus.Faulted) return;

            Logger.Error(task.Exception.StackTrace);
            MessageBox.Show(
                task.Exception.ToString(),
                "Unhandled Exception",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        });
    }
}
