namespace LijsDev.CrystalReportsRunner.Core;

using System;

/// <summary>
/// Represents an installed Crystal Reports runtime.
/// </summary>
/// <param name="version">The <see cref="Version"/> of the installed runtime.</param>
/// <param name="architecture">The CPU architecture for the runtime (for example, "x86" or "x64").</param>
public class CrystalReportsRuntime(Version version, string architecture)
{

    /// <summary>
    /// Initializes a new instance of the CrystalReportsRuntime class for the specified Crystal Reports runtime version
    /// and platform architecture.
    /// </summary>
    /// <param name="version">The version of the Crystal Reports runtime to target. This determines which runtime files will be used.</param>
    /// <param name="is64bit">Indicates whether to use the 64-bit runtime. Specify <see langword="true"/> for 64-bit; otherwise, <see
    /// langword="false"/> for 32-bit.</param>
    public CrystalReportsRuntime(Version version, bool is64bit) : this(version, is64bit ? "x64" : "x86")
    {
    }

    /// <summary>
    /// Gets the version information for the current instance.
    /// </summary>
    public Version Version { get; } = version;

    /// <summary>
    /// Gets the processor architecture for the current instance.
    /// </summary>
    public string Architecture { get; } = architecture;

    /// <summary>
    /// Gets a value indicating whether the current architecture is 64-bit.
    /// </summary>
    public bool Is64Bit => Architecture.Equals("x64", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Determines whether the specified version of the Crystal Reports runtime is installed on the system.
    /// </summary>
    /// <returns>true if the specified Crystal Reports runtime is installed; otherwise, false.</returns>
    public bool IsRuntimeInstalled() => RuntimeUtils.IsCrystalReportsRuntimeInstalled(Version, Is64Bit);
}
