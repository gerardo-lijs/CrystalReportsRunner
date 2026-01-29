namespace LijsDev.CrystalReportsRunner.Shell;

using LijsDev.CrystalReportsRunner.Core;

using System.Threading;

internal class WPFReportRunner(
    IReportViewer viewer,
    IReportExporter exporter,
    System.Windows.Threading.Dispatcher uiDispatcher,
    Shell shell) : ICrystalReportsRunner
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    /// <inheritdoc/>
    public void Print(Report report, ReportPrintOptions printOptions)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::WPFReportRunner::Print::PrinterName={printOptions.PrinterName}::Start");
        exporter.Print(report, printOptions);
        Logger.Trace($"LijsDev::CrystalReportsRunner::WPFReportRunner::Print::End");
    }

    /// <inheritdoc/>
    public void Export(
        Report report,
        ReportExportOptions reportExportOptions)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::WPFReportRunner::Export::Start");
        exporter.Export(report, reportExportOptions);
        Logger.Trace($"LijsDev::CrystalReportsRunner::WPFReportRunner::Export::End");
    }

    /// <inheritdoc/>
    public string ExportToMemoryMappedFile(
        Report report,
        ReportExportToMemoryMappedFileOptions reportExportToMemoryMappedFileOptions)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::WPFReportRunner::ExportToStream::Start");
        var mmfName = exporter.ExportToMemoryMappedFile(report, reportExportToMemoryMappedFileOptions);
        Logger.Trace($"LijsDev::CrystalReportsRunner::WPFReportRunner::ExportToStream::End");

        return mmfName;
    }

    /// <inheritdoc/>
    public void ShowReport(
        Report report,
        ReportViewerSettings viewerSettings,
        WindowHandle? owner = null)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::WPFReportRunner::ShowReport::Start");

        using var waitHandle = new ManualResetEvent(false);

        uiDispatcher.Invoke(() =>
        {
            var window = viewer.GetViewerWindow(report, viewerSettings);
            var reportFileName = report.Filename;

            window.Loaded += (sender, args) =>
            {
                Logger.Trace($"LijsDev::CrystalReportsRunner::WPFReportRunner::ShowReport::FormLoad");
                shell.FormLoaded(reportFileName, new WindowHandle(new System.Windows.Interop.WindowInteropHelper(window).EnsureHandle()));

                waitHandle.Set();
            };

            window.Closed += (sender, args) =>
            {
                Logger.Trace($"LijsDev::CrystalReportsRunner::WPFReportRunner::ShowReport::FormClosed");
                shell.FormClosed(reportFileName, GetWindowLocation(window));
            };

            if (owner is not null)
            {
                var interopHelper = new System.Windows.Interop.WindowInteropHelper(window)
                {
                    Owner = owner.Handle
                };
            }

            window.Show();

        }, null);

        waitHandle.WaitOne();

        Logger.Trace($"LijsDev::CrystalReportsRunner::WPFReportRunner::ShowReport::End");
    }

    /// <inheritdoc/>
    public bool? ShowReportDialog(
        Report report,
        ReportViewerSettings viewerSettings,
        WindowHandle owner)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::WPFReportRunner::ShowReportDialog::Start");

        using var waitHandle = new ManualResetEvent(false);
        bool? result = false;
        var reportFileName = report.Filename;

        uiDispatcher.Invoke(() =>
        {
            var window = viewer.GetViewerWindow(report, viewerSettings);

            window.Loaded += (sender, args) =>
            {
                Logger.Trace($"LijsDev::CrystalReportsRunner::WPFReportRunner::ShowReportDialog::FormLoad");
                shell.FormLoaded(reportFileName, new WindowHandle(new System.Windows.Interop.WindowInteropHelper(window).EnsureHandle()));

                waitHandle.Set();
            };

            window.Closed += (sender, args) =>
            {
                Logger.Trace($"LijsDev::CrystalReportsRunner::WPFReportRunner::ShowReportDialog::FormClosed");
                shell.FormClosed(reportFileName, GetWindowLocation(window));
            };

            var interopHelper = new System.Windows.Interop.WindowInteropHelper(window)
            {
                Owner = owner.Handle
            };
            result = window.ShowDialog();

            waitHandle.Set();
        }, null);

        waitHandle.WaitOne();

        Logger.Trace($"LijsDev::CrystalReportsRunner::WPFReportRunner::ShowReportDialog::End");
        return result;
    }

    private WindowLocation GetWindowLocation(System.Windows.Window window) =>
        new()
        {
            Width = (int)window.Width,
            Height = (int)window.Height,
            Left = (int)window.Left,
            Top = (int)window.Top,
        };
}
