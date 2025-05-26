namespace LijsDev.CrystalReportsRunner.Core;

/// <summary>
/// Crystal Reports print options
/// </summary>
public class ReportPrintOptions
{
    /// <summary>
    /// Printer name/path to print to. Null uses default printer.
    /// </summary>
    public string? PrinterName { get; set; }

    /// <summary>
    /// Gets or sets the paper orientation for printing the report (e.g., Portrait or Landscape).
    /// </summary>
    public PaperOrientations PaperOrientation { get; set; }

    /// <summary>
    /// Gets or sets the number of copies of the document to print.
    /// </summary>
    public short Copies { get; set; } = 1;

    /// <summary>
    /// Gets or sets a value indicating whether the printed document is collated.
    /// </summary>
    public bool Collated { get; set; }

    /// <summary>
    /// Gets or sets the page number of the first page to print.
    /// </summary>
    public int StartPageNumber { get; set; }

    /// <summary>
    /// Gets or sets the number of the last page to print.
    /// </summary>
    public int EndPageNumber { get; set; }
}
