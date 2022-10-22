namespace LijsDev.CrystalReportsRunner.Core;

/// <summary>
/// Crystal Reports Runner interface for named pipes communication
/// </summary>
public interface ICrystalReportsRunner
{
    // TODO: Export to Stream method for caller app to consume the PDF or Exported report without the need to write to disk/temp file.

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

    /// <summary>
    /// Show report in modeless window
    /// </summary>
    void ShowReport(
        Report report,
        ReportViewerSettings viewerSettings,
        WindowHandle? owner = null);

    /// <summary>
    /// Show report in modal window
    /// </summary>
    void ShowReportDialog(
        Report report,
        ReportViewerSettings viewSettings,
        WindowHandle owner);
}

/// <summary>
/// Crystal Reports Caller interface
/// </summary>
public interface ICrystalReportsCaller { }

/// <summary>
/// Default Crystal Reports Caller implementation
/// </summary>
public class DefaultCrystalReportsCaller : ICrystalReportsCaller { }
