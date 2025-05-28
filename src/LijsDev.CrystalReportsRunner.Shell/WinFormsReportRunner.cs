namespace LijsDev.CrystalReportsRunner.Shell;

using LijsDev.CrystalReportsRunner.Core;

using System.Threading;

internal class WinFormsReportRunner : ICrystalReportsRunner
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly IReportViewer _viewer;
    private readonly IReportExporter _exporter;
    private readonly SynchronizationContext _uiContext;
    private readonly Shell _shell;

    public WinFormsReportRunner(
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
    public void Print(Report report, ReportPrintOptions printOptions)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::Print::PrinterName={printOptions.PrinterName}::Start");
        _exporter.Print(report, printOptions);
        Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::Print::End");
    }

    /// <inheritdoc/>
    public void Export(
        Report report,
        ReportExportOptions reportExportOptions)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::Export::Start");
        _exporter.Export(report, reportExportOptions);
        Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::Export::End");
    }

    /// <inheritdoc/>
    public string ExportToMemoryMappedFile(
        Report report,
        ReportExportToMemoryMappedFileOptions reportExportToMemoryMappedFileOptions)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::ExportToStream::Start");
        var mmfName = _exporter.ExportToMemoryMappedFile(report, reportExportToMemoryMappedFileOptions);
        Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::ExportToStream::End");

        return mmfName;
    }

    /// <inheritdoc/>
    public void ShowReport(
        Report report,
        ReportViewerSettings viewerSettings,
        WindowHandle? owner = null)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::ShowReport::Start");

        using var waitHandle = new ManualResetEvent(false);

        _uiContext.Send(s =>
        {
            var form = _viewer.GetViewerForm(report, viewerSettings);
            var reportFileName = report.Filename;

            form.Load += (s, args) =>
            {
                Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::ShowReport::FormLoad");
                _shell.FormLoaded(reportFileName, new WindowHandle(form.Handle));

                waitHandle.Set();
            };

            form.FormClosed += (s, args) =>
            {
                Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::ShowReport::FormClosed");
                _shell.FormClosed(reportFileName, GetFormLocation(form));
            };

            if (owner is not null)
                form.Show(owner.GetWindow());
            else
                form.Show();

        }, null);

        waitHandle.WaitOne();

        Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::ShowReport::End");
    }

    /// <inheritdoc/>
    public bool? ShowReportDialog(
        Report report,
        ReportViewerSettings viewerSettings,
        WindowHandle owner)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::ShowReportDialog::Start");

        using var waitHandle = new ManualResetEvent(false);
        bool? result = false;
        var reportFileName = report.Filename;

        _uiContext.Send(s =>
        {
            using var form = _viewer.GetViewerForm(report, viewerSettings);

            form.Load += (s, args) =>
            {
                Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::ShowReportDialog::FormLoad");
                _shell.FormLoaded(reportFileName, new WindowHandle(form.Handle));

                waitHandle.Set();
            };

            form.FormClosed += (s, args) =>
            {
                Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::ShowReportDialog::FormClosed");
                _shell.FormClosed(reportFileName, GetFormLocation(form));
            };

            result = ConvertToBoolean(form.ShowDialog(owner.GetWindow()));

            waitHandle.Set();
        }, null);

        waitHandle.WaitOne();

        Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::ShowReportDialog::End");
        return result;
    }

    private static bool? ConvertToBoolean(DialogResult result) => result switch
    {
        DialogResult.None => null,
        DialogResult.Cancel => null,
        DialogResult.Abort => false,
        DialogResult.No => false,
        DialogResult.Ignore => false,
        DialogResult.Retry => true,
        DialogResult.OK => true,
        DialogResult.Yes => true,
        _ => throw new NotImplementedException(),
    };

    private WindowLocation GetFormLocation(Form form) =>
        new()
        {
            Width = form.Width,
            Height = form.Height,
            Left = form.Left,
            Top = form.Top
        };
}
