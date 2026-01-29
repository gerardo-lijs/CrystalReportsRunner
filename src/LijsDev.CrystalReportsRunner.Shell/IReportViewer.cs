namespace LijsDev.CrystalReportsRunner.Shell;

using LijsDev.CrystalReportsRunner.Core;

/// <summary>
/// Report Viewer interface
/// </summary>
public interface IReportViewer
{
    /// <summary>
    /// Get Crystal Reports Viewer Form
    /// </summary>
    public System.Windows.Forms.Form GetViewerForm(Report report, ReportViewerSettings settings);

    /// <summary>
    /// Get Crystal Reports Viewer Window
    /// </summary>
    public System.Windows.Window GetViewerWindow(Report report, ReportViewerSettings settings);
}
