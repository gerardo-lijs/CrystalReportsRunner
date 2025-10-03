namespace LijsDev.CrystalReportsRunner.Shell;

using System.Data;
using System.Windows;
using System.Windows.Interop;
using Core;
using NLog;

internal class WpfWindowReportRunner : ICrystalReportsRunner
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly IReportViewer _viewer;
    private readonly IReportExporter _exporter;
    private readonly SynchronizationContext _uiContext;
    private readonly Shell _shell;

    public WpfWindowReportRunner(
        IReportViewer viewer,
        IReportExporter exporter,
        SynchronizationContext uiContext,
        Shell shell)
    {
        _viewer = viewer;
        _exporter = exporter;
        _uiContext = uiContext;
        _shell = shell;
    }

    /// <inheritdoc/>
    public async Task Print(Report report, ReportPrintOptions printOptions)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::WpfWindowReportRunner::Print::PrinterName={printOptions.PrinterName}::Start");
        var dataTable = _exporter.Print(report, printOptions);
        await _shell.InvokeCallbackPipeClient(dataTable, report.Guid);
        Logger.Trace($"LijsDev::CrystalReportsRunner::WpfWindowReportRunner::Print::End");
    }

    /// <inheritdoc/>
    public void Export(
        Report report,
        ReportExportOptions reportExportOptions)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::WpfWindowReportRunner::Export::Start");
        _exporter.Export(report, reportExportOptions);
        Logger.Trace($"LijsDev::CrystalReportsRunner::WpfWindowReportRunner::Export::End");
    }

    /// <inheritdoc/>
    public string ExportToMemoryMappedFile(
        Report report,
        ReportExportToMemoryMappedFileOptions reportExportToMemoryMappedFileOptions)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::WpfWindowReportRunner::ExportToStream::Start");
        var mmfName = _exporter.ExportToMemoryMappedFile(report, reportExportToMemoryMappedFileOptions);
        Logger.Trace($"LijsDev::CrystalReportsRunner::WpfWindowReportRunner::ExportToStream::End");

        return mmfName;
    }

    /// <inheritdoc/>
    public void ShowReport(
        Report report,
        ReportViewerSettings viewerSettings,
        WindowHandle? owner = null)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::WpfWindowReportRunner::ShowReport::Start");

        using var waitHandle = new ManualResetEvent(false);

        _uiContext.Send(s =>
        {
            var window = _viewer.GetViewerWindow(report, viewerSettings);
            var reportFileName = report.Filename;

            if (window.DataContext is IReportViewerWindowVM viewerWindowVM)
            {
                viewerWindowVM.ExecuteCallbackEvent += async (sender, args) =>
                {
                    await _shell.InvokeCallbackPipeClient((DataTable)sender, args);
                };
            }

            window.Loaded += (sender, args) =>
            {
                Logger.Trace($"LijsDev::CrystalReportsRunner::WpfWindowReportRunner::ShowReport::FormLoad");
                _shell.FormLoaded(reportFileName, new WindowHandle(new WindowInteropHelper(window).EnsureHandle()));

                waitHandle.Set();
            };

            if (window.DataContext is IReportViewerWindowVM viewerWindowVM1)
            {
                window.Closed += (sender, args) =>
                {
                    Logger.Trace($"LijsDev::CrystalReportsRunner::WpfWindowReportRunner::ShowReport::FormClosed");
                    _shell.FormClosed(reportFileName, GetWindowLocation(window), viewerWindowVM1.ReportGuid);
                };
            }

            if (owner is not null)
            {
                var interopHelper = new WindowInteropHelper(window);
                interopHelper.Owner = owner.Handle;
            }

            window.Show();
        }, null);

        waitHandle.WaitOne();

        Logger.Trace($"LijsDev::CrystalReportsRunner::WpfWindowReportRunner::ShowReport::End");
    }

    /// <inheritdoc/>
    public bool? ShowReportDialog(
        Report report,
        ReportViewerSettings viewerSettings,
        WindowHandle owner)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::WpfWindowReportRunner::ShowReportDialog::Start");

        using var waitHandle = new ManualResetEvent(false);
        bool? result = false;
        var reportFileName = report.Filename;

        _uiContext.Send(s =>
        {
            var window = _viewer.GetViewerWindow(report, viewerSettings);

            if (window.DataContext is IReportViewerWindowVM viewerWindowVM)
            {
                viewerWindowVM.ExecuteCallbackEvent += async (sender, args) =>
                {
                    await _shell.InvokeCallbackPipeClient((DataTable)sender, args);
                };
            }

            window.Loaded += (sender, args) =>
            {
                Logger.Trace($"LijsDev::CrystalReportsRunner::WpfWindowReportRunner::ShowReportDialog::FormLoad");
                _shell.FormLoaded(reportFileName, new WindowHandle(new WindowInteropHelper(window).EnsureHandle()));

                waitHandle.Set();
            };

            if (window.DataContext is IReportViewerWindowVM viewerWindowVM1)
            {
                window.Closed += (sender, args) =>
                {
                    Logger.Trace($"LijsDev::CrystalReportsRunner::WpfWindowReportRunner::ShowReportDialog::FormClosed");
                    _shell.FormClosed(reportFileName, GetWindowLocation(window), viewerWindowVM1.ReportGuid);
                };
            }

            var interopHelper = new WindowInteropHelper(window);
            interopHelper.Owner = owner.Handle;

            result = window.ShowDialog();

            waitHandle.Set();
        }, null);

        waitHandle.WaitOne();

        Logger.Trace($"LijsDev::CrystalReportsRunner::WpfWindowReportRunner::ShowReportDialog::End");
        return result;
    }

    private WindowLocation GetWindowLocation(Window window) =>
        new()
        {
            Width = (int)window.Width, Height = (int)window.Height, Left = (int)window.Left, Top = (int)window.Top,
        };
}
