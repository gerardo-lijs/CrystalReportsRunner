using PipeMethodCalls;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LijsDev.CrystalReportsRunner.Abstractions
{
    public class CrystalReportsEngine : IDisposable
    {
        private readonly string _pipeName;
        private readonly PipeServerWithCallback<ICrystalReportsRunner, ICrystalReportsCaller> _pipe;
        private readonly DbConnection? _connection;

        public CrystalReportsEngine() : this(null)
        {

        }

        public CrystalReportsEngine(DbConnection? connection)
        {
            _pipeName = $"lijs-dev-crystal-reports-runner-{Guid.NewGuid()}";
            _pipe = new PipeServerWithCallback<ICrystalReportsRunner, ICrystalReportsCaller>(
                new JsonNetPipeSerializer(), _pipeName, () => new DefaultCrystalReportsCaller());
            _connection = connection;
        }

        public ReportViewerSettings ViewerSettings { get; set; } = new();


        public Task ShowReport(string path, string title)
            => ShowReport(new Report(path, title));

        public Task ShowReport(string path, string title, Dictionary<string, object> parameters)
            => ShowReport(new Report(path, title) { Parameters = parameters });

        public async Task ShowReport(
            Report report,
            WindowHandle? parent = null)
        {
            await Initialize();

            await _pipe.InvokeAsync(runner =>
                runner.ShowReport(report, ViewerSettings, parent, _connection));
        }

        public Task ShowReportDialog(string path, string title, WindowHandle parent)
          => ShowReportDialog(new Report(path, title), parent);

        public async Task ShowReportDialog(
            Report report,
            WindowHandle parent)
        {
            await Initialize();

            await _pipe.InvokeAsync(runner =>
                runner.ShowReportDialog(report, ViewerSettings, parent, _connection));
        }

        private bool _initialized = false;
        private ProcessJobTracker? _tracker;
        private Process? _process;

        private async Task Initialize()
        {
            if (!_initialized)
            {
                _tracker = new ProcessJobTracker();

                // TODO: Make this robust
                var path = "crystal-reports-runner\\CrystalReportsRunner.exe";
                var psi = new ProcessStartInfo(path)
                {
                    Arguments = $"--pipe-name {_pipeName}",
                };

                _process = new Process { StartInfo = psi };
                _process.Start();
                _tracker.AddProcess(_process);

                await _pipe.WaitForConnectionAsync();
                _initialized = true;
            }
        }

        public void Dispose()
        {
            _pipe.Dispose();
            _process?.Dispose();
            _tracker?.Dispose();
        }
    }
}
