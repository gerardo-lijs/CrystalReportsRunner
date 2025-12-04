namespace LijsDev.CrystalReportsRunner.Core;

using System;
using System.Collections.Generic;
using Microsoft.Win32;

/// <summary>
/// Provides helper methods to detect installed Crystal Reports runtimes
/// by reading the relevant registry keys on the local machine.
/// </summary>
public static class RuntimeUtils
{
    /// <summary>
    /// Checks whether the specified version of the Crystal Reports runtime is installed on the system.
    /// </summary>
    /// <param name="version">The version of the Crystal Reports runtime to check for installation.</param>
    /// <param name="is64bits">Indicates whether to check for the 64-bit runtime. Set to <see langword="true"/> to check for the 64-bit
    /// version; otherwise, checks for the 32-bit version.</param>
    /// <returns>true if the specified version and bitness of the Crystal Reports runtime is installed; otherwise, false.</returns>
    public static bool IsCrystalReportsRuntimeInstalled(Version version, bool is64bits = true)
    {
        var runtimes = EnumerateCrystalReportRuntimesInstalled();
        foreach (var runtime in runtimes)
        {
            if (runtime.Version == version && runtime.Is64Bit == is64bits)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Enumerates the installed Crystal Reports runtimes on the local machine.
    /// The method checks both 32-bit and 64-bit registry views for known
    /// Crystal Reports runtime version values and returns any discovered runtimes.
    /// </summary>
    /// <returns>A list of <see cref="CrystalReportsRuntime"/> instances found on the machine.</returns>
    public static List<CrystalReportsRuntime> EnumerateCrystalReportRuntimesInstalled()
    {
        var runtimes = new List<CrystalReportsRuntime>();

        const string subKeyPath = "SOFTWARE\\SAP BusinessObjects\\Crystal Reports for .NET Framework 4.0\\Crystal Reports";

        // Read 32-bit registry view for CRRuntime32Version
        {
            const string valueName = "CRRuntime32Version";

            using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            using var subKey = baseKey.OpenSubKey(subKeyPath);
            if (subKey is not null)
            {
                var value = subKey.GetValue(valueName) as string;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (Version.TryParse(value, out var version))
                    {
                        runtimes.Add(new CrystalReportsRuntime(version, "x86"));
                    }
                }
            }
        }

        // Read 64-bit registry view for CRRuntime32Version
        {
            const string valueName = "CRRuntime64Version";

            using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            using var subKey = baseKey.OpenSubKey(subKeyPath);
            if (subKey is not null)
            {
                var value = subKey.GetValue(valueName) as string;
                if (!string.IsNullOrWhiteSpace(value))
                {
                    if (Version.TryParse(value, out var version))
                    {
                        runtimes.Add(new CrystalReportsRuntime(version, "x64"));
                    }
                }
            }
        }

        return runtimes;
    }
}
