namespace LijsDev.CrystalReportsRunner.Core;

using Newtonsoft.Json;

/// <summary>
/// Crystal Reports Viewer Control and Window settings
/// </summary>
[Serializable]
public sealed class ReportViewerSettings
{
    /// <summary>
    /// Sets whether the report viewer window should be closed when user presses the Escape key.
    /// <para>Default: false</para>
    /// </summary>
    public bool WindowCloseOnEscapeKey { get; set; } = WindowCloseOnEscapeKeyDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeWindowCloseOnEscapeKey() => WindowCloseOnEscapeKey != WindowCloseOnEscapeKeyDefault;
    private const bool WindowCloseOnEscapeKeyDefault = false;

    /// <summary>
    /// Sets the report viewer window icon.
    /// <para>Byte array must be a valid Bitmap.</para>
    /// <para>Default: null (default icon)</para>
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public byte[]? WindowIcon { get; set; }

    /// <summary>
    /// Sets the report viewer window minimum width.
    /// <para>Default: 700</para>
    /// </summary>
    public int? WindowMinimumWidth { get; set; } = WindowMinimumWidthDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeWindowMinimumWidth() => WindowMinimumWidth != WindowMinimumWidthDefault;
    private const int WindowMinimumWidthDefault = 700;

    /// <summary>
    /// Sets the report viewer window minimum height.
    /// <para>Default: 500</para>
    /// </summary>
    public int? WindowMinimumHeight { get; set; } = WindowMinimumHeightDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeWindowMinimumHeight() => WindowMinimumHeight != WindowMinimumHeightDefault;
    private const int WindowMinimumHeightDefault = 500;

    /// <summary>
    /// Sets the report viewer window maximum width.
    /// <para>Default: null (any Width)</para>
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? WindowMaximumWidth { get; set; }

    /// <summary>
    /// Sets the report viewer window maximum height.
    /// <para>Default: null (any Height)</para>
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? WindowMaximumHeight { get; set; }

    /// <summary>
    /// Sets the report viewer window initial state.
    /// <para>Default: Maximized</para>
    /// </summary>
    public ReportViewerWindowState WindowInitialState { get; set; } = WindowInitialStateDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeWindowInitialState() => WindowInitialState != WindowInitialStateDefault;
    private const ReportViewerWindowState WindowInitialStateDefault = ReportViewerWindowState.Maximized;

    /// <summary>
    /// Sets the report viewer window starting position.
    /// <para>Default: WindowsDefaultLocation</para>
    /// </summary>
    public ReportViewerWindowStartPosition WindowInitialPosition { get; set; } = WindowInitialPositionDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeWindowInitialPosition() => WindowInitialPosition != WindowInitialPositionDefault;
    private const ReportViewerWindowStartPosition WindowInitialPositionDefault = ReportViewerWindowStartPosition.WindowsDefaultLocation;

    /// <summary>
    /// Sets whether the report viewer window is displayed in the Windows taskbar.
    /// <para>Default: true</para>
    /// </summary>
    public bool WindowShowInTaskbar { get; set; } = WindowShowInTaskbarDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeWindowShowInTaskbar() => WindowShowInTaskbar != WindowShowInTaskbarDefault;
    private const bool WindowShowInTaskbarDefault = true;

    /// <summary>
    /// Sets whether the user can minimize the report viewer window.
    /// <para>Default: true</para>
    /// </summary>
    public bool WindowAllowMinimize { get; set; } = WindowAllowMinimizeDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeWindowAllowMinimize() => WindowAllowMinimize != WindowAllowMinimizeDefault;
    private const bool WindowAllowMinimizeDefault = true;

    /// <summary>
    /// Sets whether the user can maximize the report viewer window.
    /// <para>Default: true</para>
    /// </summary>
    public bool WindowAllowMaximize { get; set; } = WindowAllowMaximizeDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeWindowAllowMaximize() => WindowAllowMaximize != WindowAllowMaximizeDefault;
    private const bool WindowAllowMaximizeDefault = true;

    /// <summary>
    /// Sets whether the user can resize the report viewer window.
    /// <para>Default: true</para>
    /// </summary>
    public bool WindowAllowResize { get; set; } = WindowAllowResizeDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeWindowAllowResize() => WindowAllowResize != WindowAllowResizeDefault;
    private const bool WindowAllowResizeDefault = true;

    /// <summary>
    /// Optionally sets the report viewer window initial height.
    /// <para>Default: null (not set)</para>
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? WindowLocationHeight { get; set; }

    /// <summary>
    /// Optionally sets the report viewer window initial width.
    /// <para>Default: null (not set)</para>
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? WindowLocationWidth { get; set; }

    /// <summary>
    /// Optionally sets the report viewer window initial left location. WindowInitialPosition may override this setting.
    /// <para>Default: null (not set)</para>
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? WindowLocationLeft { get; set; }

    /// <summary>
    /// Optionally sets the report viewer window initial top location. WindowInitialPosition may override this setting.
    /// <para>Default: null (not set)</para>
    /// </summary>
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public int? WindowLocationTop { get; set; }

    /// <summary>
    /// Set the UI culture to use in the CrystalReportViewer control.
    /// <para>Default: Use system locale</para>
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
    /// <para>Default: AllFormats</para>
    /// </summary>
    public ReportViewerExportFormats AllowedExportFormats { get; set; } = AllowedExportFormatsDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeAllowedExportFormats() => AllowedExportFormats != AllowedExportFormatsDefault;
    private const ReportViewerExportFormats AllowedExportFormatsDefault = ReportViewerExportFormats.AllFormats;

    /// <summary>
    /// Sets the report tool panel view type.
    /// <para>Default: GroupTree</para>
    /// </summary>
    public ReportViewerToolPanelViewType ToolPanelView { get; set; } = ToolPanelViewDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeToolPanelView() => ToolPanelView != ToolPanelViewDefault;
    private const ReportViewerToolPanelViewType ToolPanelViewDefault = ReportViewerToolPanelViewType.GroupTree;

    /// <summary>
    /// Sets whether the user can drill down into the report.
    /// <para>Default: true</para>
    /// </summary>
    public bool EnableDrillDown { get; set; } = EnableDrillDownDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeEnableDrillDown() => EnableDrillDown != EnableDrillDownDefault;
    private const bool EnableDrillDownDefault = true;

    /// <summary>
    /// Sets whether the refresh is enabled or not.
    /// <para>Default: true</para>
    /// </summary>
    public bool EnableRefresh { get; set; } = EnableRefreshDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeEnableRefresh() => EnableRefresh != EnableRefreshDefault;
    private const bool EnableRefreshDefault = true;

    /// <summary>
    /// Sets whether the close button on the toolbar is visible or hidden.
    /// <para>Default: true</para>
    /// </summary>
    public bool ShowCloseButton { get; set; } = ShowCloseButtonDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeShowCloseButton() => ShowCloseButton != ShowCloseButtonDefault;
    private const bool ShowCloseButtonDefault = true;

    /// <summary>
    /// Sets whether the print button on the toolbar is visible or hidden.
    /// <para>Default: true</para>
    /// </summary>
    public bool ShowPrintButton { get; set; } = ShowPrintButtonDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeShowPrintButton() => ShowPrintButton != ShowPrintButtonDefault;
    private const bool ShowPrintButtonDefault = true;

    /// <summary>
    /// Sets whether the export button on the toolbar is visible or hidden.
    /// <para>Default: true</para>
    /// </summary>
    public bool ShowExportButton { get; set; } = ShowExportButtonDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeShowExportButton() => ShowExportButton != ShowExportButtonDefault;
    private const bool ShowExportButtonDefault = true;

    /// <summary>
    /// Sets whether the zoom button on the toolbar is visible or hidden.
    /// <para>Default: true</para>
    /// </summary>
    public bool ShowZoomButton { get; set; } = ShowZoomButtonDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeShowZoomButton() => ShowZoomButton != ShowZoomButtonDefault;
    private const bool ShowZoomButtonDefault = true;

    /// <summary>
    /// Sets document zoom level. 1 = Page Width, 2 = Whole Page, or a percent number in range 25-400
    /// <para>Default: 100 %</para>
    /// </summary>
    public int ZoomLevel { get; set; } = ZoomLevelDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeZoomLevel() => ZoomLevel != ZoomLevelDefault;
    private const int ZoomLevelDefault = 100;

    /// <summary>
    /// Sets whether the copy button on the toolbar is visible or hidden.
    /// <para>Default: true</para>
    /// </summary>
    public bool ShowCopyButton { get; set; } = ShowCopyButtonDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeShowCopyButton() => ShowCopyButton != ShowCopyButtonDefault;
    private const bool ShowCopyButtonDefault = true;

    /// <summary>
    /// Sets whether the refresh button on the toolbar is visible or hidden.
    /// <para>Default: true</para>
    /// </summary>
    public bool ShowRefreshButton { get; set; } = ShowRefreshButtonDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeShowRefreshButton() => ShowRefreshButton != ShowRefreshButtonDefault;
    private const bool ShowRefreshButtonDefault = true;

    /// <summary>
    /// Sets whether the reports tabs on the toolbar are visible or hidden.
    /// <para>Default: true</para>
    /// </summary>
    public bool ShowReportTabs { get; set; } = ShowReportTabsDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeShowReportTabs() => ShowReportTabs != ShowReportTabsDefault;
    private const bool ShowReportTabsDefault = true;

    /// <summary>
    /// Sets whether the show/hide group tree button on the toolbar is visible or hidden.
    /// <para>Default: true</para>
    /// </summary>
    public bool ShowGroupTreeButton { get; set; } = ShowGroupTreeButtonDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeShowGroupTreeButton() => ShowGroupTreeButton != ShowGroupTreeButtonDefault;
    private const bool ShowGroupTreeButtonDefault = true;

    /// <summary>
    /// Sets whether the parameters panel is visible or hidden.
    /// <para>Default: true</para>
    /// </summary>
    public bool ShowParameterPanelButton { get; set; } = ShowParameterPanelButtonDefault;
    /// <inheritdoc/>
    public bool ShouldSerializeShowParameterPanelButton() => ShowParameterPanelButton != ShowParameterPanelButtonDefault;
    private const bool ShowParameterPanelButtonDefault = true;
}
