namespace LijsDev.CrystalReportsRunner.Shell;

using System.Windows.Forms;

using LijsDev.CrystalReportsRunner.Core;

/// <summary>
/// Report Viewer interface
/// </summary>
public interface IReportViewer
{
    /// <summary>
    /// Get Crystal Reports Viewer Form
    /// </summary>
    Form GetViewerForm(Report report, ReportViewerSettings settings);
}
