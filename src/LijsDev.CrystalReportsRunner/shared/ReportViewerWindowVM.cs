namespace LijsDev.CrystalReportsRunner;

using System.Drawing.Printing;
using System.IO;
using System.Resources;
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
                    //Cursor
                    //Cursor = Cursors.WaitCursor;
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

    #endregion
}
