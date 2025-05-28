namespace LijsDev.CrystalReportsRunner.Core;

/// <summary>
/// Crystal Reports export options
/// </summary>
public class ReportExportOptions
{
    /// <summary>
    /// Gets or sets the export format for the report.
    /// </summary>
    public ReportExportFormats ExportFormat { get; set; }

    /// <summary>
    /// Gets or sets the paper orientation for exporting the report (e.g., Portrait or Landscape).
    /// </summary>
    public PaperOrientations PaperOrientation { get; set; }

    /// <summary>
    /// Gets or sets the destination filename for the exported report.
    /// </summary>
    public string DestinationFilename { get; set; } = default!;

    /// <summary>
    /// Gets or sets a value indicating whether to overwrite the destination file if it already exists.
    /// Default: true
    /// </summary>
    public bool Overwrite { get; set; } = true;
}
