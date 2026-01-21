using System.Data;
using System.Windows;
using System.Windows.Threading;
using CommandLine;
using LijsDev.CrystalReportsRunner.Core;
using NLog;
using PipeMethodCalls;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace LijsDev.CrystalReportsRunner.Shell;

using LogLevel = Core.LogLevel;

/// <summary>
/// Shell implementation for Crystal Reports Runner
/// </summary>
public class Shell(IReportViewer reportViewer, IReportExporter reportExporter, Dispatcher dispatcher)
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private PipeClientWithCallback<ICrystalReportsCaller, ICrystalReportsRunner>? _pipeClient;
    private PipeClient<ICallbackDispatcher>? _callbackPipeClient;
    private Options? _options;

    public async Task StartListening(string[] args)
    {
        var result = Parser.Default.ParseArguments<Options>(args);

        if (result.Tag == ParserResultType.Parsed)
        {
            _options = result.Value;
            NLogHelper.ConfigureNLog(_options.LogDirectory, _options.LogLevel);

            Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::StartListening::Start");

            await OpenConnection();

            Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::StartListening::End");
        }
        else
        {
            Logger.Error("LijsDev::CrystalReportsRunner::Shell::ParseArguments failed with the following errors:");
            foreach (var error in result.Errors)
            {
                Logger.Error($"\t{error}");
            }

            MessageBox.Show(
                "Crystal Reports Runner is not meant to be run standalone.\n\nPlease use from caller app with NuGet package LijsDev.CrystalReportsRunner.Core.\nSee project documentation in GitHub to learn how to get started.",
                "Crystal Reports Runner",
                MessageBoxButton.OK, MessageBoxImage.Warning);

            Application.Current?.Shutdown();
        }
    }

    internal async Task InvokeCallbackPipeClient(DataTable dataTable, Guid guid)
    {
        try
        {
            await _callbackPipeClient.InvokeAsync(dispatcher => dispatcher.TryInvokeCallbackFromGuid(dataTable, guid));
        }
        catch (Exception ex)
        {
            Logger.Error(ex);
        }
    }

    internal async Task InvokePipeOnException(Exception ex)
    {
        try
        {
            if (_pipeClient is not null)
            {
                await _pipeClient.InvokeAsync(caller => caller.OnException(ex));
            }
        }
        catch (Exception exception)
        {
            Logger.Error(exception);
        }
    }

    private async Task OpenConnection()
    {
        if (_options is null) throw new NullReferenceException(nameof(_options));

        Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::StartListening::PipeName={_options.PipeName}");

        _pipeClient = new PipeClientWithCallback<ICrystalReportsCaller, ICrystalReportsRunner>(
            new JsonNetPipeSerializer(),
            ".",
            _options.PipeName,
            () => new WpfWindowReportRunner(reportViewer, reportExporter, dispatcher, this));

        _callbackPipeClient = new PipeClient<ICallbackDispatcher>(new JsonNetPipeSerializer(), _options.CallbackPipeName);

        try
        {
            Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::StartListening::ConnectAsync::Start");
            await _pipeClient.ConnectAsync();
            await _callbackPipeClient.ConnectAsync();
            Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::StartListening::ConnectAsync::End");

            Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::StartListening::WaitForRemotePipeCloseAsync::Start");
            await _pipeClient.WaitForRemotePipeCloseAsync();
            await _callbackPipeClient.WaitForRemotePipeCloseAsync();
            Logger.Trace($"LijsDev::CrystalReportsRunner::Shell::StartListening::WaitForRemotePipeCloseAsync::End");
        }
        finally
        {
            _pipeClient.Dispose();
            _callbackPipeClient.Dispose();
        }
    }

    internal async void FormClosed(string reportFileName, WindowLocation windowLocation, Guid reportGuid)
    {
        try
        {
            if (_pipeClient is not null)
            {
                await _pipeClient.InvokeAsync(caller => caller.FormClosed(reportFileName, windowLocation, reportGuid));
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed sending FormClosed event through pipe.");
        }
    }

    internal async void FormLoaded(string reportFileName, WindowHandle windowHandle)
    {
        try
        {
            if (_pipeClient is not null)
            {
                await _pipeClient.InvokeAsync(caller => caller.FormLoaded(reportFileName, windowHandle));
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed sending FormLoaded event through pipe.");
        }
    }

    public class Options
    {
        [Option('n', "pipe-name", Required = true, HelpText = "The Named Pipe this instance should connect to.")]
        public string PipeName { get; set; } = string.Empty;

        [Option('c', "callback-pipe-name", Required = true, HelpText = "The Named Pipe to use for the callbacks.")]
        public string CallbackPipeName { get; set; } = string.Empty;

        [Option("log-level", Required = false, HelpText = "Minimum logging level.")]
        public LogLevel LogLevel { get; set; } = LogLevel.Error;

        [Option("log-directory", Required = false, HelpText = "The directory for the log files.")]
        public string? LogDirectory { get; set; }
    }
}