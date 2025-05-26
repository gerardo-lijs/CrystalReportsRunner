namespace LijsDev.CrystalReportsRunner.Core;

internal class DefaultCrystalReportsCaller : ICrystalReportsCaller
{
    private readonly CrystalReportsEngine _engine;

    internal DefaultCrystalReportsCaller(CrystalReportsEngine engine)
    {
        _engine = engine;
    }

    public void FormClosed(string reportFileName, WindowLocation location) =>
        _engine.OnFormClosed(reportFileName, location);

    public void FormLoaded(string reportFileName, WindowHandle windowHandle) =>
        _engine.OnFormLoaded(reportFileName, windowHandle);
}
