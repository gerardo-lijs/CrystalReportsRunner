namespace LijsDev.CrystalReportsRunner.Core;

// NB: We define our own ViewerExportFormats enum to avoid a dependency on Crystal Reports and keep the core interface more abstract.

/// <summary>
/// Crystal Reports Viewer export formats
/// </summary>
public enum ReportViewerExportFormats
{
    /// <inheritdoc/>
    NoFormat = 0,
    /// <inheritdoc/>
    PdfFormat = 1,
    /// <inheritdoc/>
    ExcelFormat = 2,
    /// <inheritdoc/>
    WordFormat = 4,
    /// <inheritdoc/>
    RtfFormat = 8,
    /// <inheritdoc/>
    RptFormat = 16,
    /// <inheritdoc/>
    ExcelRecordFormat = 32,
    /// <inheritdoc/>
    EditableRtfFormat = 64,
    /// <inheritdoc/>
    XmlFormat = 128,
    /// <inheritdoc/>
    RptrFormat = 256,
    /// <inheritdoc/>
    XLSXFormat = 512,
    /// <inheritdoc/>
    CsvFormat = 1024,
    /// <inheritdoc/>
    XLSXPagebasedFormat = 2048,
    /// <inheritdoc/>
    XLSXRecordFormat = 4096,
    /// <inheritdoc/>
    AllFormats = 268435455
}
