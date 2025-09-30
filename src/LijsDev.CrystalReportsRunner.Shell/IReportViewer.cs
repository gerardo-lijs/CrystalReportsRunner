namespace LijsDev.CrystalReportsRunner.Shell;

using System.Windows;
using Core;

/// <summary>
/// Report Viewer interface
/// </summary>
public interface IReportViewer
{
    /// <summary>
    /// Get Crystal Reports Viewer Form
    /// </summary>
    public Form GetViewerForm(Report report, ReportViewerSettings settings);

    /// <summary>
    /// Get Crystal Reports Viewer Window
    /// </summary>
    public Window GetViewerWindow(Report report, ReportViewerSettings settings);
}
