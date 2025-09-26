namespace LijsDev.CrystalReportsRunner.Core;

using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Reflection;
using System.Text;
using PipeMethodCalls;

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
            new JsonNetPipeSerializer(), NamedPipeName, () => new DefaultCrystalReportsCaller(this));
    }

    /// <summary>
    /// Time out for waiting for child process to connect. Default: 60 seconds.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(60);

    /// <summary>
    /// The pather for the runner. By default the engine runs the first runner it finds in the main assembly directory.
    /// Full path or relative path to the assemble directory (including the filename) for the location of the executable of the runner.
    /// If you using NuGet package you can leave this with default value and it will work. Only use in advanced scenarios.
    /// Default: null - The engine uses the first runner it finds in the main assembly directory with the name 'CrystalReportsRunner.{version}.{platform}' and the runner with executable name 'LijsDev.CrystalReportsRunner.exe'
    /// </summary>
    public string? RunnerPath { get; set; }

    /// <summary>
    /// Minimum logging level for the runner. Default: Error.
    /// </summary>
    public LogLevel LogLevel { get; set; } = LogLevel.Error;

    /// <summary>
    /// Path for the log files. Default: ${specialfolder:folder=CommonApplicationData}/LijsDev/CrystalReportRunner/logs/${processname}-${shortdate}.log.
    /// For more information please refer to NLog file path documentation.
    /// Default: ProgramData/LijsDev/CrystalReportRunner/logs
    /// </summary>
    public string? LogDirectory { get; set; }

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
            runner.Export(report, new() { ExportFormat = exportFormat, DestinationFilename = destinationFilename, Overwrite = overwrite }), cancellationToken);
    }

    /// <summary>
    /// Exports a report with the specified export options.
    /// </summary>
    /// <param name="report">Report to export</param>
    /// <param name="reportExportOptions">Report export options</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
    public async Task Export(
        Report report,
        ReportExportOptions reportExportOptions,
        CancellationToken cancellationToken = default)
    {
        await Initialize(cancellationToken);

        await _pipe.InvokeAsync(runner =>
            runner.Export(report, reportExportOptions), cancellationToken);
    }

    /// <summary>
    /// Exports a report to a memory stream.
    /// </summary>
    /// <param name="report">Report to export</param>
    /// <param name="exportFormat">Export format</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
    public async Task<Stream> Export(
        Report report,
        ReportExportFormats exportFormat,
        CancellationToken cancellationToken = default)
    {
        await Initialize(cancellationToken);

        var mmfName = await _pipe.InvokeAsync(runner =>
            runner.ExportToMemoryMappedFile(report, new() { ExportFormat = exportFormat }), cancellationToken);

        var mmf = MemoryMappedFile.OpenExisting(mmfName);
        return mmf.CreateViewStream();
    }

    /// <summary>
    /// Exports a report to a memory stream with the specified export options.
    /// </summary>
    /// <param name="report">Report to export</param>
    /// <param name="reportExportToMemoryMappedFileOptions">Report export to memory mapped file options</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
    public async Task<Stream> Export(
        Report report,
        ReportExportToMemoryMappedFileOptions reportExportToMemoryMappedFileOptions,
        CancellationToken cancellationToken = default)
    {
        await Initialize(cancellationToken);

        var mmfName = await _pipe.InvokeAsync(runner =>
            runner.ExportToMemoryMappedFile(report, reportExportToMemoryMappedFileOptions), cancellationToken);

        var mmf = MemoryMappedFile.OpenExisting(mmfName);
        return mmf.CreateViewStream();
    }

    /// <summary>
    /// Prints a report to the default printer.
    /// </summary>
    /// <param name="report">Report to print</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
    public async Task Print(
        Report report,
        CancellationToken cancellationToken = default)
    {
        await Initialize(cancellationToken);

        await _pipe.InvokeAsync(runner =>
            runner.Print(report, new ReportPrintOptions()), cancellationToken);
    }

    /// <summary>
    /// Prints a report to the specified printer.
    /// </summary>
    /// <param name="report">Report to print</param>
    /// <param name="printerName">Printer name or path</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
    public async Task Print(
        Report report,
        string printerName,
        CancellationToken cancellationToken = default)
    {
        await Initialize(cancellationToken);

        await _pipe.InvokeAsync(runner =>
            runner.Print(report, new ReportPrintOptions() { PrinterName = printerName }), cancellationToken);
    }

    /// <summary>
    /// Prints a report using the specified printer options.
    /// </summary>
    /// <param name="report">Report to print</param>
    /// <param name="printOptions">Report printer options</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the work</param>
    public async Task Print(
        Report report,
        ReportPrintOptions printOptions,
        CancellationToken cancellationToken = default)
    {
        await Initialize(cancellationToken);

        await _pipe.InvokeAsync(runner =>
            runner.Print(report, printOptions), cancellationToken);
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
    public Task ShowReport(string reportFilename, string viewerTitle, Dictionary<string, object> parameters, WindowHandle? owner = null,
        CancellationToken cancellationToken = default)
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
        if (string.IsNullOrWhiteSpace(connection.Server))
            throw new Exception("Server value for database connection is required. Set connection to null if you don't want to work with a database.");
        if (string.IsNullOrWhiteSpace(connection.Database))
            throw new Exception("Database value for database connection is required. Set connection to null if you don't want to work with a database.");
        if (!connection.UseIntegratedSecurity)
        {
            if (string.IsNullOrWhiteSpace(connection.Username))
                throw new Exception("Username value for database connection must be specified when Use Integrated Security is false.");
            if (connection.Password is null)
                throw new Exception("Password value for database connection must be specified when Use Integrated Security is false.");
        }
    }

    private bool _initialized;
    private ProcessJobTracker? _tracker;
    private Process? _process;

    private async Task Initialize(CancellationToken cancellationToken)
    {
        if (!_initialized)
        {
            _tracker = new ProcessJobTracker();

            var path = GetRunnerPath(RunnerPath);

            var arguments = new List<string> { $"--pipe-name {NamedPipeName}", $"--log-level {(int)LogLevel}" };

            if (!string.IsNullOrEmpty(LogDirectory))
            {
                try
                {
                    var fullPath = Path.GetFullPath(LogDirectory);

                    if (!Directory.Exists(LogDirectory))
                    {
                        Directory.CreateDirectory(LogDirectory);
                    }

                    arguments.Add($"--log-directory {fullPath}");
                }
                catch (Exception)
                {
                    throw new InvalidOperationException("Log Directory is invalid or not-writable. Please make sure the security settings are correct.");
                }
            }

            var psi = new ProcessStartInfo(path) { Arguments = string.Join(" ", arguments) };

            // Check runner exists
            if (!File.Exists(path))
            {
                // TODO: Use custom exceptions
                throw new Exception(
                    $"Crystal Report Runner was not found in: {path}.\n\nPlease check that the Crystal Report Runner Runtime is correclty deployed via NuGet package or manually.");
            }

            _process = new Process { StartInfo = psi };
            _process.Start();
            _tracker.AddProcess(_process);

            var timeoutToken = new CancellationTokenSource(Timeout).Token;
            var compositeToken = CancellationTokenSource
                .CreateLinkedTokenSource(timeoutToken, cancellationToken).Token;

            try
            {
                await _pipe.WaitForConnectionAsync(compositeToken);
            }
            catch (OperationCanceledException)
            {
                throw new InvalidOperationException(
                    $"Connection to Runner process timed out after {Timeout.TotalSeconds:N1} seconds. Please make sure the process can run and check your antivirus settings.");
            }

            _initialized = true;
        }
    }

    private string GetRunnerPath(string? runnerPath)
    {
        var assemblyFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        assemblyFolder ??= Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (runnerPath is null && assemblyFolder is null)
            throw new InvalidProgramException(
                "Could not get assemblyFolder location with GetEntryAssembly nor GetExecutingAssembly. Please specify runnerPath explicitly with a full path.");

        List<string> candidates = [];

        // Add explicit runner path
        if (runnerPath is not null)
        {
            if (File.Exists(runnerPath))
            {
                // Fullpath provided
                candidates.Add(runnerPath);
            }
            else
            {
                // Relative path provided
                candidates.Add(Path.Combine(assemblyFolder, runnerPath));
            }
        }

        // Add subfolders in app entry assembly folder
        if (assemblyFolder is not null)
        {
            foreach (var directory in Directory.EnumerateDirectories(assemblyFolder, "CrystalReportsRunner.*"))
            {
                candidates.Add(Path.Combine(directory, "Remec.CrystalReportsRunner.exe"));
            }
        }

        var path = candidates.FirstOrDefault(File.Exists);

        if (path is null)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Crystal Report Runner was not found in: {assemblyFolder}");
            builder.AppendLine("Please check that the Crystal Report Runner Runtime is correclty deployed via NuGet package or manually.");

            if (candidates.Count > 0)
            {
                builder.AppendLine("Candidates:");

                foreach (var candidate in candidates)
                {
                    builder.AppendLine(candidate);
                }
            }
            else
            {
                builder.AppendLine($"No candidates found.");
            }

            throw new FileNotFoundException(builder.ToString());
        }

        return path;
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

    /// <summary>
    /// Fires when a form is closed.
    /// </summary>
    public event EventHandler<FormClosedEventArgs>? FormClosed;

    internal void OnFormClosed(string reportFileName, WindowLocation settings) =>
        FormClosed?.Invoke(this, new FormClosedEventArgs(reportFileName, settings));

    /// <summary>
    /// Fires when a form is loaded.
    /// </summary>
    public event EventHandler<FormLoadedEventArgs>? FormLoaded;

    internal void OnFormLoaded(string reportFileName, WindowHandle windowHandle) =>
        FormLoaded?.Invoke(this, new FormLoadedEventArgs(reportFileName, windowHandle));
}

/// <summary>
/// Form Closed Event Arguments.
/// </summary>
public class FormClosedEventArgs
{
    internal FormClosedEventArgs(string reportFileName, WindowLocation windowSettings)
    {
        ReportFileName = reportFileName;
        WindowLocation = windowSettings;
    }

    /// <summary>
    /// The report's filename which was shown in the form.
    /// </summary>
    public string ReportFileName { get; }

    /// <summary>
    /// Location of the form before it was closed.
    /// </summary>
    public WindowLocation WindowLocation { get; }
}

/// <summary>
/// Form Loaded Event Arguments.
/// </summary>
public class FormLoadedEventArgs
{
    internal FormLoadedEventArgs(string reportFileName, WindowHandle windowHandle)
    {
        ReportFileName = reportFileName;
        WindowHandle = windowHandle;
    }

    /// <summary>
    /// The report's filename which was shown in the form.
    /// </summary>
    public string ReportFileName { get; }

    /// <summary>
    /// Handle of the form.
    /// </summary>
    public WindowHandle WindowHandle { get; }
}
