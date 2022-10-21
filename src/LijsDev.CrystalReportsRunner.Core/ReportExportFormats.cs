namespace LijsDev.CrystalReportsRunner.Core;

// NB: We define our own ExportFormatType enum to avoid a dependency on Crystal Reports and keep the core interface more abstract.

/// <summary>
/// Report export formats
/// </summary>
public enum ReportExportFormats
{
    /// <summary>
    /// Crystal Reports
    /// Extension: rpt
    /// </summary>
    CrystalReport = 1,
    /// <summary>
    /// Rich Text Format (RTF)
    /// Extension: rtf
    /// </summary>
    RichText = 2,
    /// <summary>
    /// Microsoft Word (97-2003)
    /// Extension: doc
    /// </summary>
    Word = 3,
    /// <summary>
    /// Microsoft Excel (97-2003)
    /// Extension: xls
    /// </summary>
    Excel = 4,
    /// <summary>
    /// Portable Document Format
    /// Extension: pdf
    /// </summary>
    PDF = 5,
    /// <summary>
    /// HTML - not working
    /// </summary>
    HTML32 = 6,
    /// <summary>
    /// HTML - not working
    /// </summary>
    HTML40 = 7,
    /// <summary>
    /// Microsoft Excel (97-2003) Data-Only
    /// Extension: xls
    /// </summary>
    ExcelRecord = 8,
    /// <summary>
    /// Standard Text Document
    /// Extension: txt
    /// </summary>
    Text = 9,
    /// <summary>
    /// Character Separated Values (CSV)
    /// Extension: csv
    /// </summary>
    CharacterSeparatedValues = 10,
    /// <summary>
    /// Tab Separated Values (TSV)
    /// Extension: tsv
    /// </summary>
    TabSeperatedText = 11,
    /// <summary>
    /// Microsoft Word (97-2003) - Editable RTF
    /// Extension: rtf
    /// </summary>
    EditableRTF = 12,
    /// <summary>
    /// XML
    /// Extension: xml
    /// </summary>
    Xml = 13,
    /// <summary>
    /// Crystal Reports Read-Only
    /// Extension: rptr
    /// </summary>
    RPTR = 14,
    /// <summary>
    /// Microsoft Excel Workbook Data-Only
    /// Extension: xlsx
    /// </summary>
    ExcelWorkbook = 15,
}
