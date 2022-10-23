namespace WpfCaller;

using LijsDev.CrystalReportsRunner.Core;

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Interop;

public partial class MainWindow : Window
{
    /// <summary>
    /// Shared Crystal Reports Engine
    /// </summary>
    private CrystalReportsEngine? _engineInstance;

    public MainWindow()
    {
        InitializeComponent();
        Closed += MainWindow_Closed;
    }

    /// <summary>
    /// Create engine if needed for first time or if runner process no longer available.
    /// </summary>
    private void EnsureEngineAvailable()
    {
        if (_engineInstance is null)
        {
            // Create new engine if needed
            _engineInstance = CreateEngine();
        }
        else
        {
            // Create new engine if runner process is dead
            if (!_engineInstance.IsRunnerProcessAvailable())
            {
                _engineInstance.Dispose();
                _engineInstance = CreateEngine();
            }
        }
    }

    private async void MainWindow_Closed(object? sender, EventArgs e)
    {
        if (_engineInstance is not null)
        {
            await _engineInstance.CloseRunner();
            _engineInstance.Dispose();
        }
    }

    private async void ShowButton_Click(object sender, RoutedEventArgs e)
    {
        LoadingBorder.Visibility = Visibility.Visible;

        try
        {
            EnsureEngineAvailable();
            if (_engineInstance is null) throw new InvalidProgramException($"{nameof(_engineInstance)} can't be null here after calling EnsureEngineAvailable.");

            // Show
            var report = CreateReport();
            var windowHandle = new WindowHandle(new WindowInteropHelper(this).EnsureHandle());
            await _engineInstance.ShowReport(report, owner: windowHandle);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            LoadingBorder.Visibility = Visibility.Collapsed;
        }
    }

    private async void ShowDialogButton_Click(object sender, RoutedEventArgs e)
    {
        LoadingBorder.Visibility = Visibility.Visible;

        try
        {
            EnsureEngineAvailable();
            if (_engineInstance is null) throw new InvalidProgramException($"{nameof(_engineInstance)} can't be null here after calling EnsureEngineAvailable.");

            // Show
            var report = CreateReport();
            var windowHandle = new WindowHandle(new WindowInteropHelper(this).EnsureHandle());
            var result = await _engineInstance.ShowReportDialog(report, owner: windowHandle);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            LoadingBorder.Visibility = Visibility.Collapsed;
        }
    }

    private async void ExportReportButton_Click(object sender, RoutedEventArgs e)
    {
        LoadingBorder.Visibility = Visibility.Visible;

        try
        {
            EnsureEngineAvailable();
            if (_engineInstance is null) throw new InvalidProgramException($"{nameof(_engineInstance)} can't be null here after calling EnsureEngineAvailable.");

            // Export
            var report = CreateReport();
            var dstFilename = "sample_report.pdf";
            await _engineInstance.Export(report, ReportExportFormats.PDF, dstFilename, overwrite: true);

            Process.Start(new ProcessStartInfo
            {
                FileName = dstFilename,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            LoadingBorder.Visibility = Visibility.Collapsed;
        }
    }

    private async void ExportReportToStreamButton_Click(object sender, RoutedEventArgs e)
    {
        LoadingBorder.Visibility = Visibility.Visible;

        try
        {
            EnsureEngineAvailable();
            if (_engineInstance is null) throw new InvalidProgramException($"{nameof(_engineInstance)} can't be null here after calling EnsureEngineAvailable.");

            // Export
            var report = CreateReport();
            using var reportStream = await _engineInstance.Export(report, ReportExportFormats.PDF);

            var dstFilename = "sample_report_stream.pdf";
            if (System.IO.File.Exists(dstFilename)) System.IO.File.Delete(dstFilename);
            using var sw = new System.IO.FileStream(dstFilename, System.IO.FileMode.Create);
            await reportStream.CopyToAsync(sw);

            Process.Start(new ProcessStartInfo
            {
                FileName = dstFilename,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
        finally
        {
            LoadingBorder.Visibility = Visibility.Collapsed;
        }
    }

    private static Report CreateReport()
    {
        var report = new Report("SampleReport.rpt", "Sample Report")
        {
            Connection = CrystalReportsConnectionFactory.CreateSqlConnection(".\\SQLEXPRESS", "CrystalReportsSample")
        };
        report.Parameters.Add("ReportFrom", new DateTime(2022, 01, 01));
        report.Parameters.Add("UserName", "Muhammad");
        return report;
    }

    private static CrystalReportsEngine CreateEngine()
    {
        // NOTE: Create CrystalReportsSample using Schema.sql in the \samples\shared folder
        var engine = new CrystalReportsEngine();

        // Method 2: Without Connection string
        // using var engine = new CrystalReportsEngine();

        // ========== Customizing Viewer Settings (optional) ===========

        engine.ViewerSettings.AllowedExportFormats =
            ReportViewerExportFormats.PdfFormat |
            ReportViewerExportFormats.ExcelFormat |
            ReportViewerExportFormats.CsvFormat |
            ReportViewerExportFormats.WordFormat |
            ReportViewerExportFormats.XmlFormat |
            ReportViewerExportFormats.RtfFormat |
            ReportViewerExportFormats.ExcelRecordFormat |
            ReportViewerExportFormats.EditableRtfFormat |
            ReportViewerExportFormats.XLSXFormat |
            ReportViewerExportFormats.XmlFormat;

        engine.ViewerSettings.ShowReportTabs = false;
        engine.ViewerSettings.ShowRefreshButton = false;
        engine.ViewerSettings.ShowCopyButton = false;
        engine.ViewerSettings.ShowGroupTreeButton = false;
        engine.ViewerSettings.ShowParameterPanelButton = false;
        engine.ViewerSettings.EnableDrillDown = false;
        engine.ViewerSettings.ToolPanelView = ReportViewerToolPanelViewType.None;
        engine.ViewerSettings.ShowCloseButton = false;
        engine.ViewerSettings.EnableRefresh = false;

        // Set viewer Icon
        engine.ViewerSettings.WindowIcon = System.IO.File.ReadAllBytes("SampleIcon.png");

        // Optional we can also set culture for Crystal Reports Viewer UI to match the one used in your application
        //engine.ViewerSettings.SetUICulture(Thread.CurrentThread.CurrentUICulture);
        //engine.ViewerSettings.SetUICulture(System.Globalization.CultureInfo.GetCultureInfo("es-ES"));

        return engine;
    }
}
