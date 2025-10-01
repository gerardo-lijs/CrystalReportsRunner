namespace LijsDev.CrystalReportsRunner;

using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Windows;
using Core;
using CrystalDecisions.Shared;
using NLog;
using Prism.Commands;
using Prism.Mvvm;
using Resources;
using SAPBusinessObjects.WPF.Viewer;
using Shell;

/// <summary>
/// View-Model für den Report-Viewer
/// </summary>
public class ReportViewerWindowVM : BindableBase
{
    #region Members

    private string _windowTitle = "Report";
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly CustomReportDocument? _reportDocument;
    private CrystalReportsViewer? _crystalReportsViewer;
    private readonly ResourceManager _rm = Lokalisierung.ResourceManager;
    private double? _minWidth;
    private double? _minHeight;
    private double? _maxWidth;
    private double? _maxHeight;
    private double? _height;
    private double? _width;
    private double? _left;
    private double? _top;
    private ResizeMode? _resizeMode;
    private WindowState? _windowState;

    #endregion

    public DelegateCommand<CrystalReportsViewer> SetCrystalReportsViewerCommand { get; set; }
    public DelegateCommand CloseCommand { get; set; }
    public DelegateCommand PrintCommand { get; set; }

    public ReportViewerWindowVM(CustomReportDocument reportDocument, ReportViewerSettings viewerSettings)
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

        SetCrystalReportsViewerCommand = new DelegateCommand<CrystalReportsViewer>(InitializeCrystalReports);
        CloseCommand = new DelegateCommand(CloseWindow);
        PrintCommand = new DelegateCommand(Print);

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
                _crystalReportsViewer.ViewerCore.Error -= CrystalReportViewer_HandleException;
                _crystalReportsViewer.ViewerCore.Error += CrystalReportViewer_HandleException;
            }

            ConfigureReportViewer(ReportViewerSettings);
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    #region Properties

    /// <summary>
    /// Titel des Fensters
    /// </summary>
    public string WindowTitle
    {
        get => _windowTitle;
        set => SetProperty(ref _windowTitle, value);
    }

    public double? MinWidth
    {
        get => _minWidth;
        set => SetProperty(ref _minWidth, value);
    }

    public double? MinHeight
    {
        get => _minHeight;
        set => SetProperty(ref _minHeight, value);
    }

    public double? MaxWidth
    {
        get => _maxWidth;
        set => SetProperty(ref _maxWidth, value);
    }

    public double? MaxHeight
    {
        get => _maxHeight;
        set => SetProperty(ref _maxHeight, value);
    }

    public double? Height
    {
        get => _height;
        set => SetProperty(ref _height, value);
    }

    public double? Width
    {
        get => _width;
        set => SetProperty(ref _width, value);
    }

    public double? Left
    {
        get => _left;
        set => SetProperty(ref _left, value);
    }

    public double? Top
    {
        get => _top;
        set => SetProperty(ref _top, value);
    }

    public ResizeMode? ResizeMode
    {
        get => _resizeMode;
        set => SetProperty(ref _resizeMode, value);
    }

    public WindowState? WindowState
    {
        get => _windowState;
        set => SetProperty(ref _windowState, value);
    }

    public ReportViewerSettings ReportViewerSettings { get; }

    #endregion

    #region Methoden

    /// <summary>
    /// Drucken
    /// </summary>
    private void Print()
    {
        try
        {
            if (_reportDocument != null)
            {
                var printDialog = new PrintDialog();
                printDialog.AllowSomePages = true;
                //Druckereinstellungen von Report laden
                _reportDocument.LoadPrinterData(printDialog.PrinterSettings);
                var paperSource = PaperSourceKind.AutomaticFeed;
                //Sind Druckereinstellungen gültig?
                if (!printDialog.PrinterSettings.IsValid && _reportDocument.PrintOptions.PrinterName.Length > 0)
                {
                    Logger.Info(string.Format(_rm.GetString("PrinterInvalid"), _reportDocument.PrintOptions.PrinterName));
                    printDialog.Reset();
                }

                var pageRequest = new ReportPageRequestContext();
                var topage = _reportDocument.FormatEngine.GetLastPageNumber(pageRequest);
                printDialog.PrinterSettings.MaximumPage = 99999;
                printDialog.PrinterSettings.MinimumPage = 1;
                printDialog.PrinterSettings.ToPage = topage;
                printDialog.PrinterSettings.FromPage = 1;
                //Falls gültig, Papier Quelle setzen
                if (printDialog.PrinterSettings.IsValid)
                {
                    paperSource = printDialog.PrinterSettings.DefaultPageSettings.PaperSource.Kind;
                }

                //Show Print Dialog
                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    //Hat sich der Drucker geändert?
                    if (_crystalReportsViewer != null &&
                        (_reportDocument.PrintOptions.PrinterName != printDialog.PrinterSettings.PrinterName ||
                         paperSource != printDialog.PrinterSettings.DefaultPageSettings.PaperSource.Kind))
                    {
                        //Workaround, dass Filter nicht in Report gespeichert wird
                        var tempViewTimeFormula = _crystalReportsViewer.ViewerCore.ViewTimeSelectionFormula;
                        _crystalReportsViewer.ViewerCore.ViewTimeSelectionFormula =
                            _reportDocument.RecordSelectionFormula;
                        _reportDocument.SavePrinterData(printDialog.PrinterSettings);
                        _reportDocument.SaveAs(_reportDocument.FileName, false);
                        _crystalReportsViewer.ViewerCore.ViewTimeSelectionFormula = tempViewTimeFormula;
                    }

                    //Drucken
                    _reportDocument.PrintToPrinter(printDialog.PrinterSettings,
                        new PageSettings(printDialog.PrinterSettings), false);
                    //Fire Event
                    // TODO: #6648 Named Pipe
                    //if (ReportPrinted != null)
                    //{
                    //    ReportPrinted(_reportDocument.Rows, EventArgs.Empty);
                    //}
                }
            }
            else
            {
                Logger.Error(_rm.GetString("ReportNotInitialised"));
            }
        }
        catch (Exception e)
        {
            Logger.Error(e);
        }
    }

    /// <summary>
    /// Druck
    /// </summary>
    public void PrintDirect()
    {
        if (_reportDocument != null)
        {
            _reportDocument.Print();
            //Fire Event
            // TODO: #6648 Named Pipe
            //if (ReportPrinted != null)
            //{
            //    ReportPrinted(_reportDocument.Rows, EventArgs.Empty);
            //}
        }
    }

    /// <summary>
    /// Schliessen
    /// </summary>
    private void CloseWindow()
    {
        _reportDocument?.Close();
        _reportDocument?.Dispose();
    }

    /// <summary>
    /// Verarbeitet die Exception's des Crystal Report Viewer's
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    private void CrystalReportViewer_HandleException(object source, ExceptionEventArgs e)
    {
        if (_crystalReportsViewer is not null)
            _crystalReportsViewer.IsEnabled = false;
        e.Handled = true;
        Logger.Error(e.Exception);
    }

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

    #endregion
}
