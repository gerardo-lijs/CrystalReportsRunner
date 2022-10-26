namespace LijsDev.CrystalReportsRunner.Core;

/// <summary>
/// Crystal Reports Runner interface for named pipes communication
/// </summary>
public interface ICrystalReportsRunner
{
    /// <summary>
    /// Exports a report to the specified filename.
    /// </summary>
    /// <param name="report">Report to export</param>
    /// <param name="exportFormat">Export format</param>
    /// <param name="destinationFilename">Destination filename</param>
    /// <param name="overwrite">Overwrite existing destination file if exists. Default: true</param>
    void Export(
        Report report,
        ReportExportFormats exportFormat,
        string destinationFilename,
        bool overwrite = true);

    /// <summary>
    /// Exports a report to a memory mapped file.
    /// Returns the name of the memory mapped file.
    /// </summary>
    /// <param name="report">Report to export</param>
    /// <param name="exportFormat">Export format</param>
    string ExportToMemoryMappedFile(
        Report report,
        ReportExportFormats exportFormat);

    /// <summary>
    /// Show report in modeless window
    /// </summary>
    void ShowReport(
        Report report,
        ReportViewerSettings viewerSettings,
        WindowHandle? owner = null);

    /// <summary>
    /// Show report in modal window
    /// </summary>
    bool? ShowReportDialog(
        Report report,
        ReportViewerSettings viewSettings,
        WindowHandle owner);
}

/// <summary>
/// Represents location and size of window.
/// </summary>
public class WindowLocation
{
    /// <summary>
    /// Window height.
    /// </summary>
    public int Height { get; set; }
    /// <summary>
    /// Window Width.
    /// </summary>
    public int Width { get; set; }
    /// <summary>
    /// Window Top.
    /// </summary>
    public int Top { get; set; }
    /// <summary>
    /// Window Left.
    /// </summary>
    public int Left { get; set; }
}

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
    void FormClosed(string reportFileName, WindowLocation location);

    /// <summary>
    /// Form Loaded Event.
    /// </summary>
    /// <param name="reportFileName"></param>
    /// <param name="windowHandle"></param>
    void FormLoaded(string reportFileName, WindowHandle windowHandle);
}

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
