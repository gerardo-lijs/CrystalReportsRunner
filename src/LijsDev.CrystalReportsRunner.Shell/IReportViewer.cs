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

    /// <summary>
    /// Exports a report to the specified filename.
    /// </summary>
    /// <param name="report">Report to export</param>
    /// <param name="exportFormat">Export format</param>
    /// <param name="destinationFilename">Destination filename</param>
    /// <param name="overwrite">Overwrite existing destination file if exists. Default: true</param>
    void Export(
        Report report,
        ReportExportFormats exportFormat,
        string destinationFilename,
        bool overwrite = true);
}
