namespace LijsDev.CrystalReportsRunner.Core;

/// <summary>
/// Crystal Reports Viewer Control and Window settings
/// </summary>
public class ReportViewerSettings
{
    /// <summary>
    /// Sets the report viewer window minimum width
    /// </summary>
    public int? WindowMinimumWidth { get; set; } = 700;

    /// <summary>
    /// Sets the report viewer window minimum height
    /// </summary>
    public int? WindowMinimumHeight { get; set; } = 500;

    /// <summary>
    /// Sets the report viewer window initial state (Normal, Minimized or Maximized).
    /// Default: Maximized
    /// </summary>
    public ReportViewerWindowState WindowInitialState { get; set; } = ReportViewerWindowState.Maximized;

    /// <summary>
    /// Set the UI culture to use in the CrystalReportViewer control.
    /// Default: Use system locale
    /// </summary>
    public void SetUICulture(System.Globalization.CultureInfo value)
    {
        UICultureLCID = value.LCID;
    }

    /// <summary>
    /// Specified UI culture LCID.
    /// </summary>
    public int? UICultureLCID { get; set; }     // TODO: Try to make private set but test that serialization doesn't break

    /// <summary>
    /// Sets which exports formats are available.
    /// Default: AllFormats
    /// </summary>
    public ReportViewerExportFormats AllowedExportFormats { get; set; } = ReportViewerExportFormats.AllFormats;

    /// <summary>
    /// Sets the report tool panel view type.
    /// Default: GroupTree
    /// </summary>
    public ReportViewerToolPanelViewType ToolPanelView { get; set; } = ReportViewerToolPanelViewType.GroupTree;

    /// <summary>
    /// Sets whether the user can drill down into the report
    /// </summary>
    public bool EnableDrillDown { get; set; } = true;

    /// <summary>
    /// Sets whether the refresh is enabled or not.
    /// Default: true
    /// </summary>
    public bool EnableRefresh { get; set; } = true;

    /// <summary>
    /// Sets whether the close button on the toolbar is visible or hidden.
    /// Default: true
    /// </summary>
    public bool ShowCloseButton { get; set; } = true;

    /// <summary>
    /// Sets whether the print button on the toolbar is visible or hidden.
    /// Default: true
    /// </summary>
    public bool ShowPrintButton { get; set; } = true;

    /// <summary>
    /// Sets whether the export button on the toolbar is visible or hidden.
    /// Default: true
    /// </summary>
    public bool ShowExportButton { get; set; } = true;

    /// <summary>
    /// Sets whether the zoom button on the toolbar is visible or hidden.
    /// Default: true
    /// </summary>
    public bool ShowZoomButton { get; set; } = true;

    /// <summary>
    /// Sets whether the copy button on the toolbar is visible or hidden.
    /// Default: true
    /// </summary>
    public bool ShowCopyButton { get; set; } = true;

    /// <summary>
    /// Sets whether the refresh button on the toolbar is visible or hidden.
    /// Default: true
    /// </summary>
    public bool ShowRefreshButton { get; set; } = true;

    /// <summary>
    /// Sets whether the reports tabs on the toolbar are visible or hidden.
    /// Default: true
    /// </summary>
    public bool ShowReportTabs { get; set; } = true;

    /// <summary>
    /// Sets whether the show/hide group tree button on the toolbar is visible or hidden.
    /// Default: true
    /// </summary>
    public bool ShowGroupTreeButton { get; set; } = true;

    /// <summary>
    /// Sets whether the parameters panel is visible or hidden.
    /// Default: true
    /// </summary>
    public bool ShowParameterPanelButton { get; set; } = true;

    // TODO: Add Viewer Form -> StartPosition setting
}
