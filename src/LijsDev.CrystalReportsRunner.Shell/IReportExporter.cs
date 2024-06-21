namespace LijsDev.CrystalReportsRunner.Shell;

using LijsDev.CrystalReportsRunner.Core;

/// <summary>
/// Report Exporter interface
/// </summary>
public interface IReportExporter
{
    /// <summary>
    /// Prints a report to the default printer.
    /// </summary>
    /// <param name="report">Report to print</param>
    void Print(Report report);

    /// <summary>
    /// Prints a report to the default printer.
    /// </summary>
    /// <param name="report">Report to print</param>
    /// <param name="printer">Printer name or path</param>
    void PrintToPrinter(Report report, string printer);

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
    /// Exports a report to a memory mapped file.
    /// Returns the name of the memory mapped file.
    /// </summary>
    /// <param name="report">Report to export</param>
    /// <param name="exportFormat">Export format</param>
    string ExportToMemoryMappedFile(
        Report report,
        ReportExportFormats exportFormat);
}
