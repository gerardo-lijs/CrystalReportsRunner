namespace LijsDev.CrystalReportsRunner.Core;

// NB: We define our own FormStartPosition enum to avoid a dependency on either WinForms or WPF and keep the core interface more abstract.

/// <summary>
/// Specifies the initial position of a form
/// </summary>
public enum ReportViewerWindowStartPosition
{
    /// <summary>
    /// The position of the form is determined by WindowLocationLeft and WindowLocationTop properties.
    /// </summary>
    Manual,
    /// <summary>
    /// The form is centered on the current display, and has the dimensions specified in the form's size.
    /// </summary>
    CenterScreen,
    /// <summary>
    /// The form is positioned at the Windows default location and has the dimensions specified in the form's size.
    /// </summary>
    WindowsDefaultLocation,
    /// <summary>
    /// The form is positioned at the Windows default location and has the bounds determined by Windows default.
    /// </summary>
    WindowsDefaultBounds,
    /// <summary>
    /// The form is centered within the bounds of its parent form.
    /// </summary>
    CenterParent
}
