namespace LijsDev.CrystalReportsRunner.Shell;

using System.Windows.Forms;

using LijsDev.CrystalReportsRunner.Core;

/// <summary>
/// Report Viewer interface
/// </summary>
public interface IReportViewer
{
    public Form GetViewerForm(Report report, ReportViewerSettings settings, DbConnection? dbConnection);
}
