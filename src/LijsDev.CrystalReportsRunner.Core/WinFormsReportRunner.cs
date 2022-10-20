namespace LijsDev.CrystalReportsRunner.Core;

using LijsDev.CrystalReportsRunner.Abstractions;

using System;
using System.Threading;
using System.Windows.Forms;

public class WinFormsReportRunner : ICrystalReportsRunner
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
        DbConnection? dbConnection = null)
    {
        using var waitHandle = new ManualResetEvent(false);

        _runOnUIThread(() =>
        {
            var form = _viewer.GetViewerForm(report, viewerSettings, dbConnection);

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
        DbConnection? dbConnection = null)
    {
        using var waitHandle = new ManualResetEvent(false);

        _runOnUIThread(() =>
        {
            var form = _viewer.GetViewerForm(report, viewerSettings, dbConnection);
            form.StartPosition = FormStartPosition.CenterParent;

            form.ShowDialog(owner.GetWindow());

            waitHandle.Set();
        });

        waitHandle.WaitOne();
    }
}
