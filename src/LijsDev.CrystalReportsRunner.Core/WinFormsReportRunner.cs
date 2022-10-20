namespace LijsDev.CrystalReportsRunner.Core;

using LijsDev.CrystalReportsRunner.Abstractions;

using System;
using System.Data.SqlClient;
using System.Threading;
using System.Windows.Forms;

public interface IReportViewer
{
    public Form GetViewerForm(Report report, ReportViewerSettings settings, DbConnection? dbConnection);
}

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
        WindowHandle? parent = null,
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

            if (parent is not null)
            {
                form.Show(parent.GetWindow());
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
        WindowHandle parent,
        DbConnection? dbConnection = null)
    {
        using var waitHandle = new ManualResetEvent(false);

        _runOnUIThread(() =>
        {
            var form = _viewer.GetViewerForm(report, viewerSettings, dbConnection);
            form.StartPosition = FormStartPosition.CenterParent;

            form.ShowDialog(parent.GetWindow());

            waitHandle.Set();
        });

        waitHandle.WaitOne();
    }
}

public static class DbConnectionExtensions
{
    public static SqlConnectionStringBuilder GetConnectionSettings(this DbConnection dbConnection)
    {
        if (dbConnection.ConnectionString is not null)
        {
            return new SqlConnectionStringBuilder(dbConnection.ConnectionString);
        }

        return new SqlConnectionStringBuilder
        {
            DataSource = dbConnection.Server,
            UserID = dbConnection.Username,
            Password = dbConnection.Password,
            InitialCatalog = dbConnection.Database,
            IntegratedSecurity = dbConnection.UseIntegratedSecurity,
        };
    }
}


public static class WindowHandleExtensions
{
    public class Win32Window : IWin32Window
    {
        public IntPtr Handle { get; }

        public Win32Window(IntPtr handle)
        {
            Handle = handle;
        }
    }

    public static IWin32Window GetWindow(this WindowHandle handle)
    {
        return new Win32Window(handle.Handle);
    }
}
