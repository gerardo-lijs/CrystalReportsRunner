namespace LijsDev.CrystalReportsRunner.Abstractions;

/// <summary>
/// Crystal Reports Viewer Control and Window settings
/// </summary>
public class ReportViewerSettings
{
    /// <summary>
    /// Set the UI culture to use in the CrystalReportViewer control.
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
    public CrystalReportsViewerExportFormats AllowedExportFormats { get; set; } = CrystalReportsViewerExportFormats.AllFormats;

    /// <summary>
    /// Sets the report tool panel view type.
    /// Default: GroupTree
    /// </summary>
    public CrystalReportsToolPanelViewType ToolPanelView { get; set; } = CrystalReportsToolPanelViewType.GroupTree;

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
    /// Sets whether the CrystalReportViewer control's toolbar has the button for closing a report page
    /// Default: true
    /// </summary>
    public bool ShowCloseButton { get; set; } = true;

    /// <summary>
    /// Sets whether the print button on the toolbar is visible or hidden.
    /// Default: true
    /// </summary>
    public bool ShowPrintButton { get; set; } = true;

    /// <summary>
    /// Sets whether the CrystalReportViewer control's toolbar has the button for exporting.
    /// Default: true
    /// </summary>
    public bool ShowExportButton { get; set; } = true;

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
    /// Sets whether the CrystalReportViewer control's toolbar has the button for showing or hiding the group tree
    /// Default: true
    /// </summary>
    public bool ShowGroupTreeButton { get; set; } = true;

    /// <summary>
    /// Sets whether the parameters panel is visible or hidden.
    /// Default: true
    /// </summary>
    public bool ShowParameterPanelButton { get; set; } = true;
}
