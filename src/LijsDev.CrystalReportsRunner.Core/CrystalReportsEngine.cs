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
    private readonly string _pipeName;
    private readonly PipeServerWithCallback<ICrystalReportsRunner, ICrystalReportsCaller> _pipe;
    private readonly CrystalReportsConnection? _connection;

    /// <inheritdoc/>
    public CrystalReportsEngine() : this(null)
    {
    }

    /// <inheritdoc/>
    public CrystalReportsEngine(CrystalReportsConnection? connection)
    {
        // Validate CrystalReportsConnection
        if (connection is not null)
        {
            // TODO: Use custom Exceptions
            if (string.IsNullOrWhiteSpace(connection.Server)) throw new Exception("Server value for database connection is required. Set connection to null if you don't want to work with a database.");
            if (string.IsNullOrWhiteSpace(connection.Database)) throw new Exception("Database value for database connection is required. Set connection to null if you don't want to work with a database.");
            if (!connection.UseIntegratedSecurity)
            {
                if (string.IsNullOrWhiteSpace(connection.Username)) throw new Exception("Username value for database connection must be specified when Use Integrated Security is false.");
                if (connection.Password is null) throw new Exception("Password value for database connection must be specified when Use Integrated Security is false.");
            }
        }

        _pipeName = $"lijs-dev-crystal-reports-runner-{Guid.NewGuid()}";
        _pipe = new PipeServerWithCallback<ICrystalReportsRunner, ICrystalReportsCaller>(
            new JsonNetPipeSerializer(), _pipeName, () => new DefaultCrystalReportsCaller());
        _connection = connection;
    }

    /// <summary>
    /// Viewer settings
    /// </summary>
    public ReportViewerSettings ViewerSettings { get; set; } = new();

    /// <summary>
    /// Show specified Crystal Reports file in Viewer window
    /// </summary>
    /// <param name="reportFilename">Crystal Reports RPT file path</param>
    /// <param name="viewerTitle">Title to display in the Viewer window</param>
    /// <param name="owner">Optional owner window handle. Useful for CenterParent initial location</param>
    public Task ShowReport(string reportFilename, string viewerTitle, WindowHandle? owner = null)
        => ShowReport(new Report(reportFilename, viewerTitle), owner);

    /// <summary>
    /// Show specified Crystal Reports file in Viewer window
    /// </summary>
    /// <param name="reportFilename">Crystal Reports RPT file path</param>
    /// <param name="viewerTitle">Title to display in the Viewer window</param>
    /// <param name="parameters">Database query parameters</param>
    /// <param name="owner">Optional owner window handle. Useful for CenterParent initial location</param>
    public Task ShowReport(string reportFilename, string viewerTitle, Dictionary<string, object> parameters, WindowHandle? owner = null)
        => ShowReport(new Report(reportFilename, viewerTitle) { Parameters = parameters }, owner);

    /// <summary>
    /// Show specified Crystal Reports in Viewer window
    /// </summary>
    /// <param name="report">Report to show</param>
    /// <param name="owner">Optional owner window handle. Useful for CenterParent initial location</param>
    public async Task ShowReport(
        Report report,
        WindowHandle? owner = null)
    {
        await Initialize();

        await _pipe.InvokeAsync(runner =>
            runner.ShowReport(report, ViewerSettings, owner, _connection));
    }

    /// <summary>
    /// Show specified Crystal Reports file in Viewer dialog
    /// </summary>
    /// <param name="reportFilename">Crystal Reports RPT file path</param>
    /// <param name="viewerTitle">Title to display in the Viewer window</param>
    /// <param name="owner">Owner window handle</param>
    public Task ShowReportDialog(string reportFilename, string viewerTitle, WindowHandle owner)
      => ShowReportDialog(new Report(reportFilename, viewerTitle), owner);

    /// <summary>
    /// Show specified Crystal Reports in Viewer dialog
    /// </summary>
    /// <param name="report">Report to show</param>
    /// <param name="owner">Owner window handle</param>
    public async Task ShowReportDialog(
        Report report,
        WindowHandle owner)
    {
        await Initialize();

        await _pipe.InvokeAsync(runner =>
            runner.ShowReportDialog(report, ViewerSettings, owner, _connection));
    }

    private bool _initialized = false;
    private ProcessJobTracker? _tracker;
    private Process? _process;

    private async Task Initialize()
    {
        if (!_initialized)
        {
            _tracker = new ProcessJobTracker();

            // TODO: Make this robust using app location and not relative with issue with WorkingDir
            // TODO: Allow to specify another location for user that deploy manually
            var path = "crystal-reports-runner\\LijsDev.CrystalReportsRunner.exe";
            var psi = new ProcessStartInfo(path)
            {
                Arguments = $"--pipe-name {_pipeName}",
            };

            // Check runner exists
            if (!File.Exists(path))
            {
                // TODO: Use custom exceptions
                throw new Exception($"Crystal Report Runner was not found in: {path}.\n\nPlease check that the Crystal Report Runner Runtime is correclty deployed via NuGet package or manually.");
            }

            _process = new Process { StartInfo = psi };
            _process.Start();
            _tracker.AddProcess(_process);

            await _pipe.WaitForConnectionAsync();
            _initialized = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _pipe.Dispose();
        _process?.Dispose();
        _tracker?.Dispose();
    }
}