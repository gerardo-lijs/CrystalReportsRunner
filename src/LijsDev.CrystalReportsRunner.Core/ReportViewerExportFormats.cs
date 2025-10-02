namespace LijsDev.CrystalReportsRunner.Core;

// NB: We define our own ViewerExportFormats enum to avoid a dependency on Crystal Reports and keep the core interface more abstract.

/// <summary>
/// Crystal Reports Viewer export formats
/// </summary>
[Flags]
public enum ReportViewerExportFormats
{
    /// <summary>
    /// Export disabled
    /// </summary>
    NoFormat = 0,

    /// <summary>
    /// Portable Document Format
    /// </summary>
    PdfFormat = 1,

    /// <summary>
    /// Microsoft Excel (97-2003) - XLS
    /// </summary>
    ExcelFormat = 2,

    /// <summary>
    /// Microsoft Word (97-2003) - DOC
    /// </summary>
    WordFormat = 4,

    /// <summary>
    /// Rich Text Format (RTF)
    /// </summary>
    RtfFormat = 8,

    /// <summary>
    /// Crystal Reports
    /// </summary>
    RptFormat = 0x10,

    /// <summary>
    /// Microsoft Excel (97-2003) Data-Only - XLS
    /// </summary>
    ExcelRecordFormat = 0x20,

    /// <summary>
    /// Microsoft Word (97-2003) - Editable RTF
    /// </summary>
    EditableRtfFormat = 0x40,

    /// <summary>
    /// XML
    /// </summary>
    XmlFormat = 0x80,

    /// <summary>
    /// Microsoft Excel Workbook Data-Only - XLSX
    /// </summary>
    XLSXFormat = 0x200,

    /// <summary>
    /// Character Separated Values (CSV)
    /// </summary>
    CsvFormat = 0x400,

    /// <summary>
    /// All available formats
    /// </summary>
    AllFormats = 0xFFFFFFF,
}
