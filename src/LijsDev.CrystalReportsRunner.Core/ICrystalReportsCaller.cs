namespace LijsDev.CrystalReportsRunner.Core;

/// <summary>
/// Crystal Reports Caller interface
/// </summary>
public interface ICrystalReportsCaller
{
    /// <summary>
    /// Form Closed Event
    /// </summary>
    /// <param name="reportFileName"></param>
    /// <param name="location"></param>
    /// <param name="reportGuid"></param>
    public void FormClosed(string reportFileName, WindowLocation location, Guid reportGuid);

    /// <summary>
    /// Form Loaded Event.
    /// </summary>
    /// <param name="reportFileName"></param>
    /// <param name="windowHandle"></param>
    public void FormLoaded(string reportFileName, WindowHandle windowHandle);

    /// <summary>
    /// Exceptions from the CrystalReportsViewer are propagated trough here.
    /// </summary>
    /// <param name="ex"></param>
    public void OnException(Exception ex);
}
