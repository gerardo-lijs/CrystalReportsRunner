namespace LijsDev.CrystalReportsRunner.Shell;

/// <summary>
/// Interface for ReportViewerWindowVM to avoid dependencies between the projects.
/// </summary>
public interface IReportViewerWindowVM
{
    /// <summary>
    /// This event propagates Exceptions from the ReportViewer to the engine.
    /// </summary>
    event EventHandler<Exception>? ReportViewerExceptionEvent;

    /// <summary>
    /// This event tells listeners to execute their callbacks.
    /// </summary>
    event EventHandler<Guid>? ExecuteCallbackEvent;

    /// <summary>
    /// This is the Guid associated with the Report opened in the Window.
    /// </summary>
    Guid ReportGuid { get; }
}