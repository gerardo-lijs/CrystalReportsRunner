namespace LijsDev.CrystalReportsRunner.Shell;

using CommandLine;

using LijsDev.CrystalReportsRunner.Core;

using PipeMethodCalls;

using System;
using System.Threading;
using System.Windows.Forms;

/// <summary>
/// Shell implementation for Crystal Reports Runner
/// </summary>
public class Shell
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    /// <inheritdoc/>
    public class Options
    {
        /// <inheritdoc/>
        [Option('n', "pipe-name", Required = true, HelpText = "The Named Pipe this instance should connect to.")]
        public string PipeName { get; set; } = string.Empty;
    }

    private readonly IReportViewer _reportViewer;
    private readonly IReportExporter _reportExporter;

    /// <inheritdoc/>
    public Shell(IReportViewer reportViewer,
        IReportExporter reportExporter)
    {
        _reportViewer = reportViewer;
        _reportExporter = reportExporter;
    }

    private Options? _options;

    private void ThreadExceptionHandler(object s, ThreadExceptionEventArgs e)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::ThreadExceptionHandler");
        Logger.Fatal(e.Exception);
        Application.ExitThread();
    }

    private async void StartUpHandler(object s, EventArgs e)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::StartUpHandler");
        Application.Idle -= StartUpHandler;

        // Capture WindowsFormsSynchronizationContext UI context
        var uiContext = SynchronizationContext.Current ?? throw new Exception($"{nameof(StartUpHandler)} needs to be run from a UI Thread.");
        await OpenConnection(uiContext);
    }

    /// <inheritdoc/>
    public void StartListening(string[] args)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::StartListening::Start");
        var result = Parser.Default.ParseArguments<Options>(args);

        if (result.Tag == ParserResultType.Parsed)
        {
            _options = result.Value;
            Application.ThreadException += ThreadExceptionHandler;
            Application.Idle += StartUpHandler;
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::StartListening::Application.Run");
                Application.Run();
            }
            finally
            {
                Application.Idle -= StartUpHandler;
            }
        }
        else
        {
            Logger.Error("LijsDev::CrystalReportsRunner::Shell::ParseArguments failed with the following errors:");
            foreach (var error in result.Errors)
            {
                Logger.Error($"\t{error}");
            }
            MessageBox.Show("Crystal Reports Runner is not meant to be run standalone.\n\nPlease use from caller app with NuGet package LijsDev.CrystalReportsRunner.Core.\nSee project documentation in GitHub to learn how to get started.", "Crystal Reports Runner",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::StartListening::End");
    }

    private async Task OpenConnection(SynchronizationContext uiContext)
    {
        if (_options is null) throw new NullReferenceException(nameof(_options));

        Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::StartListening::PipeName={_options.PipeName}");
        using var pipeClient = new PipeClientWithCallback<ICrystalReportsCaller, ICrystalReportsRunner>(
                 new JsonNetPipeSerializer(),
                 ".",
        _options.PipeName,
                 () => new WinFormsReportRunner(_reportViewer, _reportExporter, uiContext));

        Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::StartListening::ConnectAsync::Start");
        await pipeClient.ConnectAsync();
        Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::StartListening::ConnectAsync::End");

        Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::StartListening::WaitForRemotePipeCloseAsync::Start");
        await pipeClient.WaitForRemotePipeCloseAsync();
        Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::StartListening::WaitForRemotePipeCloseAsync::End");

        Application.ExitThread();
    }
}
