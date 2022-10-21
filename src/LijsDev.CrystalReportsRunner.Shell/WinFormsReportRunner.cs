namespace LijsDev.CrystalReportsRunner.Shell;

using LijsDev.CrystalReportsRunner.Core;

using System;
using System.Threading;
using System.Windows.Forms;

internal class WinFormsReportRunner : ICrystalReportsRunner
{
    private readonly IReportViewer _viewer;
    private readonly Action<Action> _runOnUIThread;

    public WinFormsReportRunner(
        IReportViewer viewer,
        Action<Action> runOnUIThread)
    {
        _viewer = viewer;
        _runOnUIThread = runOnUIThread;
    }

    public void Export(
        Report report,
        ReportViewerExportFormats exportFormat,
        string destinationFilename,
        bool overwrite = true)
    {
    }

    public void ShowReport(
        Report report,
        ReportViewerSettings viewerSettings,
        WindowHandle? owner = null)
    {
        using var waitHandle = new ManualResetEvent(false);

        _runOnUIThread(() =>
        {
            var form = _viewer.GetViewerForm(report, viewerSettings);
            form.Load += (s, args) =>
            {
                waitHandle.Set();
            };

            if (owner is not null)
                form.Show(owner.GetWindow());
            else
                form.Show();

            // TODO: We might want to expose the Window Location and State somehow to the caller app once the user closes so it could be saved for interface settings in following executions.
            // TODO: Add call to Dispose form when closed?
        });

        waitHandle.WaitOne();
    }

    public void ShowReportDialog(
        Report report,
        ReportViewerSettings viewerSettings,
        WindowHandle owner)
    {
        using var waitHandle = new ManualResetEvent(false);

        _runOnUIThread(() =>
        {
            using var form = _viewer.GetViewerForm(report, viewerSettings);
            form.ShowDialog(owner.GetWindow());

            // TODO: We might want to expose the Window Location and State somehow to the caller app once the user closes so it could be saved for interface settings in following executions.

            waitHandle.Set();
        });

        waitHandle.WaitOne();
    }
}
