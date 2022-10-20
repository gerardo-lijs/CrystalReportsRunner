namespace LijsDev.CrystalReportsRunner.Abstractions;

public class ReportViewerSettings
{
    public int? ProductLacaleLCID { get; set; }
    public CrystalReportsViewerExportFormats AllowedExportFormats { get; set; } = CrystalReportsViewerExportFormats.AllFormats;

    public bool ShowRefreshButton { get; set; } = true;
    public bool ShowCopyButton { get; set; } = true;
    public bool ShowGroupTreeButton { get; set; } = true;
    public bool ShowParameterPanelButton { get; set; } = true;
    public CrystalReportsToolPanelViewType ToolPanelView { get; set; } = CrystalReportsToolPanelViewType.GroupTree;
    public bool EnableDrillDown { get; set; } = true;
    public bool EnableRefresh { get; set; } = true;
    public bool ShowCloseButton { get; set; } = true;
}

