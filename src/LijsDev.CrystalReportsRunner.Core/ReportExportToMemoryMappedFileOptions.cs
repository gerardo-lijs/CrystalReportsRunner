namespace LijsDev.CrystalReportsRunner.Core;

/// <summary>
/// Crystal Reports export to memory mapped file options
/// </summary>
public class ReportExportToMemoryMappedFileOptions
{
    /// <summary>
    /// Gets or sets the export format for the report.
    /// </summary>
    public ReportExportFormats ExportFormat { get; set; }

    /// <summary>
    /// Gets or sets the paper orientation for exporting the report (e.g., Portrait or Landscape).
    /// </summary>
    public PaperOrientations PaperOrientation { get; set; }
}
