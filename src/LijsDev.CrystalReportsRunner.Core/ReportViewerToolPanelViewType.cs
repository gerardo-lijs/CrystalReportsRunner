namespace LijsDev.CrystalReportsRunner.Core;

// NB: We define our own ToolPanelViewType enum to avoid a dependency on Crystal Reports and keep the core interface more abstract.

/// <summary>
/// Crystal Reports Viewer Tool Panel View Types
/// </summary>
public enum ReportViewerToolPanelViewType
{
    /// <inheritdoc/>
    None = 0,
    /// <inheritdoc/>
    GroupTree = 1,
    /// <inheritdoc/>
    ParameterPanel = 2
}
