namespace LijsDev.CrystalReportsRunner.Core;

// NB: We define our own PaperOrientations enum to avoid a dependency on Crystal Reports and keep the core interface more abstract.

/// <summary>
/// Crystal Reports print options paper orientations
/// </summary>
public enum PaperOrientations
{
    /// <inheritdoc/>
    DefaultPaperOrientation = 0,
    /// <inheritdoc/>
    Portrait = 1,
    /// <inheritdoc/>
    Landscape = 2
}
