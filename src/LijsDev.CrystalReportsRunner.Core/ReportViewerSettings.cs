namespace LijsDev.CrystalReportsRunner.Core;

using Newtonsoft.Json;

/// <summary>
/// Crystal Reports Viewer Control and Window settings
/// </summary>
[Serializable]
public sealed class ReportViewerSettings
{
    // TODO: Add WindowCloseWithEscapeKey  setting

    /// <summary>
    /// Sets the report viewer window icon.
    /// Byte array must be a valid Bitmap.
    /// Default: null (default icon)
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public byte[]? WindowIcon { get; set; }

    /// <summary>
    /// Sets the report viewer window minimum width.
    /// Default: 700
    /// </summary>
    public int? WindowMinimumWidth { get; set; } = WindowMinimumWidthDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeWindowMinimumWidth() => WindowMinimumWidth != WindowMinimumWidthDefault;
    private const int WindowMinimumWidthDefault = 700;

    /// <summary>
    /// Sets the report viewer window minimum height.
    /// Default: 500
    /// </summary>
    public int? WindowMinimumHeight { get; set; } = WindowMinimumHeightDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeWindowMinimumHeight() => WindowMinimumHeight != WindowMinimumHeightDefault;
    private const int WindowMinimumHeightDefault = 500;

    /// <summary>
    /// Sets the report viewer window maximum width.
    /// Default: null (any Width)
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? WindowMaximumWidth { get; set; }

    /// <summary>
    /// Sets the report viewer window maximum height.
    /// Default: null (any Height)
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? WindowMaximumHeight { get; set; }

    /// <summary>
    /// Sets the report viewer window initial state.
    /// Default: Maximized
    /// </summary>
    public ReportViewerWindowState WindowInitialState { get; set; } = WindowInitialStateDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeWindowInitialState() => WindowInitialState != WindowInitialStateDefault;
    private const ReportViewerWindowState WindowInitialStateDefault = ReportViewerWindowState.Maximized;

    /// <summary>
    /// Sets the report viewer window starting position.
    /// Default: WindowsDefaultLocation
    /// </summary>
    public ReportViewerWindowStartPosition WindowInitialPosition { get; set; } = WindowInitialPositionDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeWindowInitialPosition() => WindowInitialPosition != WindowInitialPositionDefault;
    private const ReportViewerWindowStartPosition WindowInitialPositionDefault = ReportViewerWindowStartPosition.WindowsDefaultLocation;

    /// <summary>
    /// Sets whether the report viewer window is displayed in the Windows taskbar.
    /// Default: true
    /// </summary>
    public bool WindowShowInTaskbar { get; set; } = WindowShowInTaskbarDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeWindowShowInTaskbar() => WindowShowInTaskbar != WindowShowInTaskbarDefault;
    private const bool WindowShowInTaskbarDefault = true;

    /// <summary>
    /// Sets whether the user can minimize the report viewer window.
    /// Default: true
    /// </summary>
    public bool WindowAllowMinimize { get; set; } = WindowAllowMinimizeDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeWindowAllowMinimize() => WindowAllowMinimize != WindowAllowMinimizeDefault;
    private const bool WindowAllowMinimizeDefault = true;

    /// <summary>
    /// Sets whether the user can maximize the report viewer window.
    /// Default: true
    /// </summary>
    public bool WindowAllowMaximize { get; set; } = WindowAllowMaximizeDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeWindowAllowMaximize() => WindowAllowMaximize != WindowAllowMaximizeDefault;
    private const bool WindowAllowMaximizeDefault = true;

    /// <summary>
    /// Sets whether the user can resize the report viewer window.
    /// Default: true
    /// </summary>
    public bool WindowAllowResize { get; set; } = WindowAllowResizeDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeWindowAllowResize() => WindowAllowResize != WindowAllowResizeDefault;
    private const bool WindowAllowResizeDefault = true;

    /// <summary>
    /// Optionally sets the report viewer window initial left location. WindowInitialPosition may override this setting.
    /// Default: null (not set)
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? WindowLocationLeft { get; set; }

    /// <summary>
    /// Optionally sets the report viewer window initial top location. WindowInitialPosition may override this setting.
    /// Default: null (not set)
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? WindowLocationTop { get; set; }

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
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? UICultureLCID { get; set; }

    /// <summary>
    /// Sets which exports formats are available.
    /// Default: AllFormats
    /// </summary>
    public ReportViewerExportFormats AllowedExportFormats { get; set; } = AllowedExportFormatsDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeAllowedExportFormats() => AllowedExportFormats != AllowedExportFormatsDefault;
    private const ReportViewerExportFormats AllowedExportFormatsDefault = ReportViewerExportFormats.AllFormats;

    /// <summary>
    /// Sets the report tool panel view type.
    /// Default: GroupTree
    /// </summary>
    public ReportViewerToolPanelViewType ToolPanelView { get; set; } = ToolPanelViewDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeToolPanelView() => ToolPanelView != ToolPanelViewDefault;
    private const ReportViewerToolPanelViewType ToolPanelViewDefault = ReportViewerToolPanelViewType.GroupTree;

    /// <summary>
    /// Sets whether the user can drill down into the report.
    /// Default: true
    /// </summary>
    public bool EnableDrillDown { get; set; } = EnableDrillDownDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeEnableDrillDown() => EnableDrillDown != EnableDrillDownDefault;
    private const bool EnableDrillDownDefault = true;

    /// <summary>
    /// Sets whether the refresh is enabled or not.
    /// Default: true
    /// </summary>
    public bool EnableRefresh { get; set; } = EnableRefreshDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeEnableRefresh() => EnableRefresh != EnableRefreshDefault;
    private const bool EnableRefreshDefault = true;

    /// <summary>
    /// Sets whether the close button on the toolbar is visible or hidden.
    /// Default: true
    /// </summary>
    public bool ShowCloseButton { get; set; } = ShowCloseButtonDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeShowCloseButton() => ShowCloseButton != ShowCloseButtonDefault;
    private const bool ShowCloseButtonDefault = true;

    /// <summary>
    /// Sets whether the print button on the toolbar is visible or hidden.
    /// Default: true
    /// </summary>
    public bool ShowPrintButton { get; set; } = ShowPrintButtonDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeShowPrintButton() => ShowPrintButton != ShowPrintButtonDefault;
    private const bool ShowPrintButtonDefault = true;

    /// <summary>
    /// Sets whether the export button on the toolbar is visible or hidden.
    /// Default: true
    /// </summary>
    public bool ShowExportButton { get; set; } = ShowExportButtonDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeShowExportButton() => ShowExportButton != ShowExportButtonDefault;
    private const bool ShowExportButtonDefault = true;

    /// <summary>
    /// Sets whether the zoom button on the toolbar is visible or hidden.
    /// Default: true
    /// </summary>
    public bool ShowZoomButton { get; set; } = ShowZoomButtonDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeShowZoomButton() => ShowZoomButton != ShowZoomButtonDefault;
    private const bool ShowZoomButtonDefault = true;

    /// <summary>
    /// Sets whether the copy button on the toolbar is visible or hidden.
    /// Default: true
    /// </summary>
    public bool ShowCopyButton { get; set; } = ShowCopyButtonDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeShowCopyButton() => ShowCopyButton != ShowCopyButtonDefault;
    private const bool ShowCopyButtonDefault = true;

    /// <summary>
    /// Sets whether the refresh button on the toolbar is visible or hidden.
    /// Default: true
    /// </summary>
    public bool ShowRefreshButton { get; set; } = ShowRefreshButtonDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeShowRefreshButton() => ShowRefreshButton != ShowRefreshButtonDefault;
    private const bool ShowRefreshButtonDefault = true;

    /// <summary>
    /// Sets whether the reports tabs on the toolbar are visible or hidden.
    /// Default: true
    /// </summary>
    public bool ShowReportTabs { get; set; } = ShowReportTabsDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeShowReportTabs() => ShowReportTabs != ShowReportTabsDefault;
    private const bool ShowReportTabsDefault = true;

    /// <summary>
    /// Sets whether the show/hide group tree button on the toolbar is visible or hidden.
    /// Default: true
    /// </summary>
    public bool ShowGroupTreeButton { get; set; } = ShowGroupTreeButtonDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeShowGroupTreeButton() => ShowGroupTreeButton != ShowGroupTreeButtonDefault;
    private const bool ShowGroupTreeButtonDefault = true;

    /// <summary>
    /// Sets whether the parameters panel is visible or hidden.
    /// Default: true
    /// </summary>
    public bool ShowParameterPanelButton { get; set; } = ShowParameterPanelButtonDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeShowParameterPanelButton() => ShowParameterPanelButton != ShowParameterPanelButtonDefault;
    private const bool ShowParameterPanelButtonDefault = true;
}
