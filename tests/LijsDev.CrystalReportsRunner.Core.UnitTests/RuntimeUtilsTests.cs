namespace LijsDev.CrystalReportsRunner.Core.UnitTests;

using Xunit;

public class RuntimeUtilsTests
{
    [Fact]
    public void EnumerateCrystalReportRuntimesInstalled_ShouldWork()
    {
        var runtimes = LijsDev.CrystalReportsRunner.Core.RuntimeUtils.EnumerateCrystalReportRuntimesInstalled();

        // Can't assert much here, as it depends on the environment the tests are run in.
        // Used interactively to see what runtimes are installed.
    }

    [Fact]
    public void IsCrystalReportsRuntimeInstalled_ShouldWork()
    {
        var runtimeInstalled = LijsDev.CrystalReportsRunner.Core.RuntimeUtils.IsCrystalReportsRuntimeInstalled(new Version(13, 0, 35), is64bits: false);

        // Can't assert much here, as it depends on the environment the tests are run in.
        // Used interactively to see what runtimes are installed.
    }
}
