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

    private readonly ManualResetEvent _waitHandle = new(false);
    private readonly IReportViewer _reportViewer;
    private readonly IReportExporter _reportExporter;
    private Form? _mainForm;

    /// <inheritdoc/>
    public Shell(IReportViewer reportViewer,
        IReportExporter reportExporter)
    {
        _reportViewer = reportViewer;
        _reportExporter = reportExporter;
    }

    /// <inheritdoc/>
    public void StartListening(string[] args)
    {
        var result = Parser.Default.ParseArguments<Options>(args);

        if (result.Tag == ParserResultType.Parsed)
        {
            var thread = new Thread(() => OpenConnection(result.Value));
            thread.Start();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            _mainForm = CreateInvisibleForm();

            Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::StartListening::_waitHandle.Set");
            _waitHandle.Set();

            Application.Run(_mainForm);
        }
        else
        {
            Logger.Error("LijsDev::CrystalReportsRunner::Shell::ParseArguments failed with the following errors:");
            foreach (var error in result.Errors)
            {
                Logger.Error($"\t{error}");
            }
        }
    }

    private Form CreateInvisibleForm()
    {
        return new Form
        {
            Width = 0,
            Height = 0,
            ShowInTaskbar = false,
            FormBorderStyle = FormBorderStyle.None,
            Opacity = 0,
            WindowState = FormWindowState.Minimized,
        };
    }

    /// <inheritdoc/>
    public void RunCodeOnUIThread(Action action)
    {
        if (_mainForm is not null)
        {
            _mainForm.BeginInvoke(action);
        }
    }

    private async void OpenConnection(Options options)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::OpenConnection::PipeName={options.PipeName}");

        using var pipeClient = new PipeClientWithCallback<ICrystalReportsCaller, ICrystalReportsRunner>(
                 new JsonNetPipeSerializer(),
                 ".",
                 options.PipeName,
                 () => new WinFormsReportRunner(_reportViewer, _reportExporter, RunCodeOnUIThread));

        Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::OpenConnection::WaitOne::Start");
        _waitHandle.WaitOne();
        Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::OpenConnection::WaitOne::End");

        Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::OpenConnection::ConnectAsync::Start");
        await pipeClient.ConnectAsync();
        Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::OpenConnection::ConnectAsync::End");

        Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::OpenConnection::WaitForRemotePipeCloseAsync::Start");
        await pipeClient.WaitForRemotePipeCloseAsync();
        Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::OpenConnection::WaitForRemotePipeCloseAsync::End");
    }
}
