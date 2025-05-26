namespace LijsDev.CrystalReportsRunner.Core;

/// <summary>
/// Crystal Reports Runner interface for named pipes communication
/// </summary>
public interface ICrystalReportsRunner
{
    /// <summary>
    /// Prints a report.
    /// </summary>
    /// <param name="report">Report to print</param>
    /// <param name="printerName">Printer name/path to print to. Null uses default printer.</param>
    public void Print(Report report, string? printerName = null);

    /// <summary>
    /// Prints a report using the specified printer options.
    /// </summary>
    /// <param name="report">Report to print</param>
    /// <param name="printOptions">Report printer options</param>
    public void PrintWithOptions(Report report, ReportPrintOptions printOptions);

    /// <summary>
    /// Exports a report to the specified filename.
    /// </summary>
    /// <param name="report">Report to export</param>
    /// <param name="exportFormat">Export format</param>
    /// <param name="destinationFilename">Destination filename</param>
    /// <param name="overwrite">Overwrite existing destination file if exists. Default: true</param>
    public void Export(
        Report report,
        ReportExportFormats exportFormat,
        string destinationFilename,
        bool overwrite = true);

    /// <summary>
    /// Exports a report to a memory mapped file.
    /// Returns the name of the memory mapped file.
    /// </summary>
    /// <param name="report">Report to export</param>
    /// <param name="exportFormat">Export format</param>
    public string ExportToMemoryMappedFile(
        Report report,
        ReportExportFormats exportFormat);

    /// <summary>
    /// Show report in modeless window
    /// </summary>
    public void ShowReport(
        Report report,
        ReportViewerSettings viewerSettings,
        WindowHandle? owner = null);

    /// <summary>
    /// Show report in modal window
    /// </summary>
    public bool? ShowReportDialog(
        Report report,
        ReportViewerSettings viewSettings,
        WindowHandle owner);
}
