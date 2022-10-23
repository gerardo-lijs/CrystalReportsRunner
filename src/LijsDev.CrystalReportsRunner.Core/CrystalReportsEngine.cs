namespace LijsDev.CrystalReportsRunner.Core;

using PipeMethodCalls;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

/// <summary>
/// Crystal Reports Engine
/// </summary>
public sealed class CrystalReportsEngine : IDisposable
{
    private readonly PipeServerWithCallback<ICrystalReportsRunner, ICrystalReportsCaller> _pipe;

    /// <inheritdoc/>
    public CrystalReportsEngine()
    {
        NamedPipeName = $"lijs-dev-crystal-reports-runner-{Guid.NewGuid()}";
        _pipe = new PipeServerWithCallback<ICrystalReportsRunner, ICrystalReportsCaller>(
            new JsonNetPipeSerializer(), NamedPipeName, () => new DefaultCrystalReportsCaller());
    }

    /// <summary>
    /// Viewer settings
    /// </summary>
    public ReportViewerSettings ViewerSettings { get; set; } = new();

    /// <summary>
    /// Named pipes name used by the runner process to communicate with CrystalReportsEngine.
    /// </summary>
    public string NamedPipeName { get; }

    /// <summary>
    /// Returns true if Crystal Reports Runner process/pipe is alive (connected state).
    /// </summary>
    /// <returns></returns>
    public bool IsRunnerProcessAvailable() => _pipe.State == PipeState.Connected;

    /// <summary>
    /// Exports a report to the specified filename.
    /// </summary>
    /// <param name="report">Report to export</param>
    /// <param name="exportFormat">Export format</param>
    /// <param name="destinationFilename">Destination filename</param>
    /// <param name="overwrite">Overwrite existing destination file if exists. Default: true</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
    public async Task Export(
        Report report,
        ReportExportFormats exportFormat,
        string destinationFilename,
        bool overwrite = true,
        CancellationToken cancellationToken = default)
    {
        await Initialize(cancellationToken);

        await _pipe.InvokeAsync(runner =>
            runner.Export(report, exportFormat, destinationFilename, overwrite), cancellationToken);
    }

    /// <summary>
    /// Show specified Crystal Reports file in Viewer window.
    /// Viewer will close when CrystalReportEngine is disposed unless CloseRunnerProcessAtExit is set to false.
    /// </summary>
    /// <param name="reportFilename">Crystal Reports RPT file path</param>
    /// <param name="viewerTitle">Title to display in the Viewer window</param>
    /// <param name="owner">Optional owner window handle. Useful for CenterParent initial location</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
    public Task ShowReport(string reportFilename, string viewerTitle, WindowHandle? owner = null, CancellationToken cancellationToken = default)
        => ShowReport(new Report(reportFilename, viewerTitle), owner, cancellationToken);

    /// <summary>
    /// Show specified Crystal Reports file in Viewer window.
    /// Viewer will close when CrystalReportEngine is disposed unless CloseRunnerProcessAtExit is set to false.
    /// </summary>
    /// <param name="reportFilename">Crystal Reports RPT file path</param>
    /// <param name="viewerTitle">Title to display in the Viewer window</param>
    /// <param name="parameters">Database query parameters</param>
    /// <param name="owner">Optional owner window handle. Useful for CenterParent initial location</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
    public Task ShowReport(string reportFilename, string viewerTitle, Dictionary<string, object> parameters, WindowHandle? owner = null, CancellationToken cancellationToken = default)
        => ShowReport(new Report(reportFilename, viewerTitle) { Parameters = parameters }, owner, cancellationToken);

    /// <summary>
    /// Show specified Crystal Reports in Viewer window.
    /// Viewer will close when CrystalReportEngine is disposed unless CloseRunnerProcessAtExit is set to false.
    /// </summary>
    /// <param name="report">Report to show</param>
    /// <param name="owner">Optional owner window handle. Useful for CenterParent initial location</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
    public async Task ShowReport(
        Report report,
        WindowHandle? owner = null,
        CancellationToken cancellationToken = default)
    {
        ValidateReportConnection(report.Connection);
        await Initialize(cancellationToken);

        await _pipe.InvokeAsync(runner =>
            runner.ShowReport(report, ViewerSettings, owner), cancellationToken);
    }

    /// <summary>
    /// Show specified Crystal Reports file in Viewer dialog
    /// </summary>
    /// <param name="reportFilename">Crystal Reports RPT file path</param>
    /// <param name="viewerTitle">Title to display in the Viewer window</param>
    /// <param name="owner">Owner window handle</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
    public Task ShowReportDialog(string reportFilename, string viewerTitle, WindowHandle owner, CancellationToken cancellationToken = default)
      => ShowReportDialog(new Report(reportFilename, viewerTitle), owner, cancellationToken);

    /// <summary>
    /// Show specified Crystal Reports in Viewer dialog
    /// </summary>
    /// <param name="report">Report to show</param>
    /// <param name="owner">Owner window handle</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
    /// <returns>The dialog result.</returns>
    public async Task<bool?> ShowReportDialog(
        Report report,
        WindowHandle owner,
        CancellationToken cancellationToken = default)
    {
        ValidateReportConnection(report.Connection);
        await Initialize(cancellationToken);

        return await _pipe.InvokeAsync(runner =>
             runner.ShowReportDialog(report, ViewerSettings, owner), cancellationToken);
    }

    private void ValidateReportConnection(CrystalReportsConnection? connection)
    {
        if (connection is null) return;

        // TODO: Use custom Exceptions
        if (string.IsNullOrWhiteSpace(connection.Server)) throw new Exception("Server value for database connection is required. Set connection to null if you don't want to work with a database.");
        if (string.IsNullOrWhiteSpace(connection.Database)) throw new Exception("Database value for database connection is required. Set connection to null if you don't want to work with a database.");
        if (!connection.UseIntegratedSecurity)
        {
            if (string.IsNullOrWhiteSpace(connection.Username)) throw new Exception("Username value for database connection must be specified when Use Integrated Security is false.");
            if (connection.Password is null) throw new Exception("Password value for database connection must be specified when Use Integrated Security is false.");
        }
    }

    private bool _initialized;
    private ProcessJobTracker? _tracker;
    private int? _processId;
    private Process? _process;

    private async Task Initialize(CancellationToken cancellationToken)
    {
        if (!_initialized)
        {
            _tracker = new ProcessJobTracker();

            // TODO: Make this robust using app location and not relative with issue with WorkingDir
            // TODO: Allow to specify another location for user that deploy manually
            var path = "crystal-reports-runner\\LijsDev.CrystalReportsRunner.exe";
            var psi = new ProcessStartInfo(path)
            {
                Arguments = $"--pipe-name {NamedPipeName}",
            };

            // Check runner exists
            if (!File.Exists(path))
            {
                // TODO: Use custom exceptions
                throw new Exception($"Crystal Report Runner was not found in: {path}.\n\nPlease check that the Crystal Report Runner Runtime is correclty deployed via NuGet package or manually.");
            }

            _process = new Process { StartInfo = psi };
            _process.Start();
            _processId = _process.Id;
            _tracker.AddProcess(_process);

            await _pipe.WaitForConnectionAsync(cancellationToken);
            _initialized = true;
        }
    }

    /// <summary>
    /// Nicely close pipe and wait a little bit for it to end gracefully and avoid killing it with process tracker.
    /// </summary>
    public async Task CloseRunner(int waitForCloseMilliseconds = 500)
    {
        _pipe.Dispose();
        await Task.Delay(waitForCloseMilliseconds);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // Nicely close pipe and wait a little bit for it to end gracefully and avoid killing it with process tracker.
        _pipe.Dispose();
        Thread.Sleep(500);

        // Dispose process with tracker
        _process?.Dispose();
        _tracker?.Dispose();
    }
}
