namespace LijsDev.CrystalReportsRunner.Shell;

using LijsDev.CrystalReportsRunner.Core;

/// <summary>
/// Report Exporter interface
/// </summary>
public interface IReportExporter
{
    /// <summary>
    /// Prints a report using the specified printer options.
    /// </summary>
    /// <param name="report">Report to print</param>
    /// <param name="printOptions">Report printer options</param>
    public void Print(
        Report report,
        ReportPrintOptions printOptions);

    /// <summary>
    /// Exports a report to the specified filename.
    /// </summary>
    /// <param name="report">Report to export</param>
    /// <param name="reportExportOptions">Report export options</param>
    public void Export(
        Report report,
        ReportExportOptions reportExportOptions);

    /// <summary>
    /// Exports a report to a memory mapped file.
    /// Returns the name of the memory mapped file.
    /// </summary>
    /// <param name="report">Report to export</param>
    /// <param name="reportExportToMemoryMappedFileOptions">Report export to memory mapped file options</param>
    public string ExportToMemoryMappedFile(
        Report report,
        ReportExportToMemoryMappedFileOptions reportExportToMemoryMappedFileOptions);
}
