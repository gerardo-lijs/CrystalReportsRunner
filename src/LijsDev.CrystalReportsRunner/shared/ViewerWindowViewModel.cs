namespace LijsDev.CrystalReportsRunner;

using System.Globalization;
using System.IO;
using System.Windows;

using SAPBusinessObjects.WPF.Viewer;
using NLog;

using LijsDev.CrystalReportsRunner.Core;
using CrystalDecisions.CrystalReports.Engine;

/// <summary>
/// View-Model für den Report-Viewer
/// </summary>
internal class ViewerWindowViewModel
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly ReportDocument? _reportDocument;
    private CrystalReportsViewer? _crystalReportsViewer;

    public ViewerWindowViewModel(ReportDocument reportDocument, ReportViewerSettings viewerSettings)
    {
        _reportDocument = reportDocument;
        ReportViewerSettings = viewerSettings;

        if (_reportDocument != null)
        {
            WindowTitle = _reportDocument.SummaryInfo.ReportTitle ?? "";
            if (WindowTitle.Length == 0)
            {
                WindowTitle = Path.GetFileName(_reportDocument.FileName);
            }
        }

        //SetCrystalReportsViewerCommand = new DelegateCommand<CrystalReportsViewer>(InitializeCrystalReports);
        //CloseCommand = new DelegateCommand(CloseWindow);
        //PrintCommand = new DelegateCommand(Print);

        ConfigureReportViewerWindow(ReportViewerSettings);
    }

    private void InitializeCrystalReports(CrystalReportsViewer crystalReportsViewer)
    {
        try
        {
            _crystalReportsViewer = crystalReportsViewer;

            if (_reportDocument != null)
            {
                _crystalReportsViewer.ViewerCore.ReportSource = _reportDocument;
            }

            ConfigureReportViewer(ReportViewerSettings);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    //public DelegateCommand<CrystalReportsViewer> SetCrystalReportsViewerCommand { get; set; }
    //public DelegateCommand CloseCommand { get; set; }
    //public DelegateCommand PrintCommand { get; set; }

    public string WindowTitle { get; set; } = "Report";

    public double? MinWidth { get; set; }

    public double? MinHeight { get; set; }

    public double? MaxWidth { get; set; }

    public double? MaxHeight { get; set; }

    public double? Height { get; set; }

    public double? Width { get; set; }

    public double? Left { get; set; }

    public double? Top { get; set; }

    public ResizeMode? ResizeMode { get; set; }

    public WindowState? WindowState { get; set; }

    public ReportViewerSettings ReportViewerSettings { get; }

    private void ConfigureReportViewer(ReportViewerSettings settings)
    {
        // Settings on Crystal Reports Viewer
        if (_crystalReportsViewer is null) return;

        if (settings.UICultureLCID is not null)
        {
            _crystalReportsViewer.ViewerCore.ProductLocale = new CultureInfo(settings.UICultureLCID.Value);
        }

        _crystalReportsViewer.ViewerCore.AllowedExportFormats = (int)settings.AllowedExportFormats;
        _crystalReportsViewer.ViewerCore.EnableDrillDown = settings.EnableDrillDown;
        _crystalReportsViewer.ViewerCore.EnableRefresh = settings.EnableRefresh;

        _crystalReportsViewer.ShowRefreshButton = settings.ShowRefreshButton;
        _crystalReportsViewer.ViewerCore.EnableRefresh = settings.EnableRefresh;
        _crystalReportsViewer.ShowCopyButton = settings.ShowCopyButton;
        _crystalReportsViewer.ShowPrintButton = settings.ShowPrintButton;
        _crystalReportsViewer.ShowExportButton = settings.ShowExportButton;
    }

    private void ConfigureReportViewerWindow(ReportViewerSettings settings)
    {
        MinWidth = settings.WindowMinimumWidth;

        MinHeight = settings.WindowMinimumHeight;

        if (settings.WindowMaximumWidth is not null)
        {
            MaxWidth = settings.WindowMaximumWidth > settings.WindowMinimumWidth ? settings.WindowMaximumWidth.Value : double.PositiveInfinity;
        }

        if (settings.WindowMaximumHeight is not null)
        {
            MaxHeight = settings.WindowMaximumHeight > settings.WindowMinimumHeight ? settings.WindowMaximumHeight.Value : double.PositiveInfinity;
        }

        Height = settings.WindowLocationHeight;

        Width = settings.WindowLocationWidth;

        Left = settings.WindowLocationLeft;

        Top = settings.WindowLocationTop;

        if ((settings.WindowAllowMaximize && settings.WindowAllowMinimize) || settings.WindowAllowResize)
        {
            ResizeMode = System.Windows.ResizeMode.CanResize;
        }
        else if (!settings.WindowAllowMaximize && settings.WindowAllowMinimize)
        {
            ResizeMode = System.Windows.ResizeMode.CanMinimize;
        }
        else if (settings.WindowAllowMinimize)
        {
            ResizeMode = System.Windows.ResizeMode.CanMinimize;
        }
        else
        {
            ResizeMode = System.Windows.ResizeMode.NoResize;
        }

        WindowState = MapReportViewerWindowStateToWindowState(settings.WindowInitialState);
    }

    private WindowState MapReportViewerWindowStateToWindowState(ReportViewerWindowState state)
    {
        return state switch
        {
            ReportViewerWindowState.Normal => System.Windows.WindowState.Normal,
            ReportViewerWindowState.Minimized => System.Windows.WindowState.Minimized,
            ReportViewerWindowState.Maximized => System.Windows.WindowState.Maximized,
            _ => throw new ArgumentOutOfRangeException(nameof(state)),
        };
    }
}
