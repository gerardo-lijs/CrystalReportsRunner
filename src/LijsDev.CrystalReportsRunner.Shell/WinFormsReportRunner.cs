namespace LijsDev.CrystalReportsRunner.Shell;

using LijsDev.CrystalReportsRunner.Core;

using System.Threading;

internal class WinFormsReportRunner : ICrystalReportsRunner
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private readonly IReportViewer _viewer;
    private readonly IReportExporter _exporter;
    private readonly SynchronizationContext _uiContext;

    public WinFormsReportRunner(
        IReportViewer viewer,
        IReportExporter exporter,
        SynchronizationContext uiContext)
    {
        _viewer = viewer;
        _exporter = exporter;
        _uiContext = uiContext;
    }

    public void Export(
        Report report,
        ReportExportFormats exportFormat,
        string destinationFilename,
        bool overwrite = true)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::Export::Start");
        _exporter.Export(report, exportFormat, destinationFilename, overwrite);
        Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::Export::End");
    }

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
            form.Load += (s, args) =>
            {
                Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::ShowReport::FormLoad");
                waitHandle.Set();
            };
            form.FormClosed += (s, args) =>
            {
                Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::ShowReport::FormClosed");
                Application.ExitThread();
            };

            if (owner is not null)
                form.Show(owner.GetWindow());
            else
                form.Show();

            // TODO: We might want to expose the Window Location and State somehow to the caller app once the user closes so it could be saved for interface settings in following executions.
            // TODO: Add call to Dispose form when closed?
        }, null);

        waitHandle.WaitOne();

        Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::ShowReport::End");
    }

    public void ShowReportDialog(
        Report report,
        ReportViewerSettings viewerSettings,
        WindowHandle owner)
    {
        Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::ShowReportDialog::Start");

        using var waitHandle = new ManualResetEvent(false);

        _uiContext.Send(s =>
        {
            using var form = _viewer.GetViewerForm(report, viewerSettings);
            form.ShowDialog(owner.GetWindow());

            // TODO: We might want to expose the Window Location and State somehow to the caller app once the user closes so it could be saved for interface settings in following executions.

            waitHandle.Set();
        }, null);

        waitHandle.WaitOne();

        Logger.Trace($"LijsDev::CrystalReportsRunner::WinFormsReportRunner::ShowReportDialog::End");
    }
}
