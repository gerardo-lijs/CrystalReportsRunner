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

    public void ShowReport(
        Report report,
        ReportViewerSettings viewerSettings,
        WindowHandle? owner = null,
        CrystalReportsConnection? dbConnection = null)
    {
        using var waitHandle = new ManualResetEvent(false);

        _runOnUIThread(() =>
        {
            var form = _viewer.GetViewerForm(report, viewerSettings, dbConnection);

            // TODO: Add ViewerSettings for StartPosition
            form.StartPosition = FormStartPosition.CenterScreen;

            form.Load += (s, args) =>
            {
                waitHandle.Set();
            };

            if (owner is not null)
            {
                form.Show(owner.GetWindow());
            }
            else
            {
                form.Show();
            }
        });

        waitHandle.WaitOne();
    }

    public void ShowReportDialog(
        Report report,
        ReportViewerSettings viewerSettings,
        WindowHandle owner,
        CrystalReportsConnection? dbConnection = null)
    {
        using var waitHandle = new ManualResetEvent(false);

        _runOnUIThread(() =>
        {
            var form = _viewer.GetViewerForm(report, viewerSettings, dbConnection);

            // TODO: Add ViewerSettings for StartPosition
            form.StartPosition = FormStartPosition.CenterParent;

            form.ShowDialog(owner.GetWindow());

            waitHandle.Set();
        });

        waitHandle.WaitOne();
    }
}
