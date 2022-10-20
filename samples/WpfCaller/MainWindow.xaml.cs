namespace WpfCaller;

using LijsDev.CrystalReportsRunner.Abstractions;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

public partial class MainWindow : Window
{
    private readonly List<IDisposable> _disposables = new();

    public MainWindow()
    {
        InitializeComponent();
        Closed += MainWindow_Closed;
    }

    private void MainWindow_Closed(object? sender, EventArgs e)
    {
        foreach (var disposable in _disposables.ToArray())
        {
            _disposables.Remove(disposable);
            disposable.Dispose();
        }
    }

    private async void ShowButton_Click(object sender, RoutedEventArgs e)
    {
        LoadingBorder.Visibility = Visibility.Visible;

        try
        {
            var engine = CreateEngine();
            _disposables.Add(engine);
            var report = CreateReport();

            await engine.ShowReport(report);
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
            using var engine = CreateEngine();
            var report = CreateReport();

            var windowHandle = new WindowHandle(new WindowInteropHelper(this).EnsureHandle());
            await engine.ShowReportDialog(report, owner: windowHandle);
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
        var report = new Report("SampleReport.rpt", "Sample Report");
        report.Parameters.Add("ReportFrom", new DateTime(2022, 01, 01));
        report.Parameters.Add("UserName", "Muhammad");
        return report;
    }

    private static CrystalReportsEngine CreateEngine()
    {
        // NOTE: Create CrystalReportsSample using Schema.sql in the \samples\shared folder
        var connection = new DbConnection("Server=.\\SQLEXPRESS;Database=CrystalReportsSample;Trusted_Connection=True;");
        var engine = new CrystalReportsEngine(connection);

        // Method 2: Without Connection string
        // using var engine = new CrystalReportsEngine();

        // ========== Customizing Viewer Settings ===========

        engine.ViewerSettings.AllowedExportFormats =
            CrystalReportsViewerExportFormats.PdfFormat |
            CrystalReportsViewerExportFormats.ExcelFormat |
            CrystalReportsViewerExportFormats.CsvFormat |
            CrystalReportsViewerExportFormats.WordFormat |
            CrystalReportsViewerExportFormats.XmlFormat |
            CrystalReportsViewerExportFormats.RtfFormat |
            CrystalReportsViewerExportFormats.ExcelRecordFormat |
            CrystalReportsViewerExportFormats.EditableRtfFormat |
            CrystalReportsViewerExportFormats.XLSXFormat |
            CrystalReportsViewerExportFormats.XmlFormat;

        engine.ViewerSettings.ShowRefreshButton = false;
        engine.ViewerSettings.ShowCopyButton = false;
        engine.ViewerSettings.ShowGroupTreeButton = false;
        engine.ViewerSettings.ShowParameterPanelButton = false;
        engine.ViewerSettings.EnableDrillDown = false;
        engine.ViewerSettings.ToolPanelView = CrystalReportsToolPanelViewType.None;
        engine.ViewerSettings.ShowCloseButton = false;
        engine.ViewerSettings.EnableRefresh = false;

        engine.ViewerSettings.ProductLocaleLCID = Thread.CurrentThread.CurrentUICulture.LCID;
        return engine;
    }
}
