namespace LijsDev.CrystalReportsRunner.Core;

// NB: We define our own WindowState enum to avoid a dependency on either WinForms or WPF and keep the core interface more abstract.

/// <summary>
/// Specifies how a form window is displayed.
/// </summary>
public enum ReportViewerWindowState
{
    /// <summary>
    /// A default sized window.
    /// </summary>
    Normal,
    /// <summary>
    /// A minimized window
    /// </summary>
    Minimized,
    /// <summary>
    /// A maximized window.
    /// </summary>
    Maximized
}
