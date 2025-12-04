namespace LijsDev.CrystalReportsRunner.Core;

using System;

/// <summary>
/// Represents an installed Crystal Reports runtime.
/// </summary>
/// <param name="Version">The <see cref="Version"/> of the installed runtime.</param>
/// <param name="Architecture">The CPU architecture for the runtime (for example, "x86" or "x64").</param>
public record CrystalReportsRuntime(Version Version, string Architecture)
{
    /// <summary>
    /// Gets a value indicating whether the current architecture is 64-bit.
    /// </summary>
    public bool Is64Bit => Architecture.Equals("x64", StringComparison.OrdinalIgnoreCase);
}
