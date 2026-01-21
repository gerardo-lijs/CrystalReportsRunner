namespace LijsDev.CrystalReportsRunner.Core;

internal class DefaultCrystalReportsCaller : ICrystalReportsCaller
{
    private readonly CrystalReportsEngine _engine;

    internal DefaultCrystalReportsCaller(CrystalReportsEngine engine)
    {
        _engine = engine;
    }

    public void FormClosed(string reportFileName, WindowLocation location, Guid reportGuid) =>
        _engine.OnFormClosed(reportFileName, location, reportGuid);

    public void FormLoaded(string reportFileName, WindowHandle windowHandle) =>
        _engine.OnFormLoaded(reportFileName, windowHandle);

    public void OnException(Exception ex) =>
        _engine.OnException(ex);
}
