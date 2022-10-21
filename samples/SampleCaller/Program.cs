using LijsDev.CrystalReportsRunner.Core;

// ========== Initializing Engine ===========

// Method 1: With Connection string
using var engine = new CrystalReportsEngine();

// Method 2: Without Connection string
// using var engine = new CrystalReportsEngine();

// ========== Customizing Viewer Settings ===========

engine.ViewerSettings.AllowedExportFormats =
    ReportViewerExportFormats.PdfFormat |
    ReportViewerExportFormats.ExcelFormat |
    ReportViewerExportFormats.CsvFormat |
    ReportViewerExportFormats.WordFormat |
    ReportViewerExportFormats.XmlFormat |
    ReportViewerExportFormats.RtfFormat |
    ReportViewerExportFormats.ExcelRecordFormat |
    ReportViewerExportFormats.EditableRtfFormat |
    ReportViewerExportFormats.XLSXFormat |
    ReportViewerExportFormats.XmlFormat;

engine.ViewerSettings.ShowRefreshButton = false;
engine.ViewerSettings.ShowCopyButton = false;
engine.ViewerSettings.ShowGroupTreeButton = false;
engine.ViewerSettings.ShowParameterPanelButton = false;
engine.ViewerSettings.EnableDrillDown = false;
engine.ViewerSettings.ToolPanelView = ReportViewerToolPanelViewType.None;
engine.ViewerSettings.ShowCloseButton = false;
engine.ViewerSettings.EnableRefresh = false;

engine.ViewerSettings.SetUICulture(Thread.CurrentThread.CurrentUICulture);

// ========== Showing the Report ===========

// Method 1: Full Control
var report = new Report("SampleReport.rpt", "Sample Report")
{
    Connection = CrystalReportsConnectionFactory.CreateSqlConnection(".\\SQLEXPRESS", "CrystalReportsSample")
};
report.Parameters.Add("ReportFrom", new DateTime(2022, 01, 01));
report.Parameters.Add("UserName", "Gerardo");

await engine.ShowReport(report);

// Method 2: Easy
await engine.ShowReport("SampleReport.rpt", "Sample Report", new Dictionary<string, object>
{
    { "ReportFrom", new DateTime(2022, 01, 01) },
    { "UserName",  "Gerardo" },
});

Console.ReadKey();
