namespace LijsDev.CrystalReportsRunner.Shell;

using System.Drawing.Printing;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using PaperSource = CrystalDecisions.Shared.PaperSource;

/// <summary>
/// Report-Document mit Erweiterungen
/// </summary>
public class CustomReportDocument : ReportDocument
{
    private string _origRecordSelectionFormula = string.Empty;

    /// <summary>
    /// Lädt den angegebenen Report
    /// </summary>
    /// <param name="filename">Dateiname</param>
    public override void Load(string filename)
    {
        base.Load(filename);
        _origRecordSelectionFormula = RecordSelectionFormula;
    }

    /// <summary>
    /// Speichert den angegebenen Report
    /// </summary>
    /// <param name="filename">Dateiname</param>
    public override void SaveAs(string filename)
    {
        if (_origRecordSelectionFormula != RecordSelectionFormula)
        {
            RecordSelectionFormula = _origRecordSelectionFormula;
        }

        //Falls Datei nicht schreibgeschützt ist, speichern
        filename = filename.Replace("rassdk://", "");
        if ((File.GetAttributes(filename) & FileAttributes.ReadOnly) !=
            FileAttributes.ReadOnly)
        {
            base.SaveAs(filename);
        }
    }

    /// <summary>
    /// Report-Titel
    /// </summary>
    public string ReportTitle
    {
        get => SummaryInfo.ReportTitle;
        set => SummaryInfo.ReportTitle = value;
    }

    /// <summary>
    /// Setzt die Parameterwerte
    /// </summary>
    /// <param name="name">Name</param>
    /// <param name="value">Wert</param>
    public void SetParameterValueIfExists(string name, object value)
    {
        var par = ParameterFields.Find(name, "");
        if (par != null)
        {
            SetParameterValue(name, value);
        }
    }

    /// <summary>
    /// Druckt den Report
    /// </summary>
    public void Print()
    {
        var ps = new PrinterSettings();
        var pd = new PrintDocument();
        LoadPrinterData(ps);
        //Standarddrucker nehmen, falls der aktuelle Drucker ungültig ist
        if (!ps.IsValid)
        {
            ps.PrinterName = pd.PrinterSettings.PrinterName;
        }

        var req = new ReportPageRequestContext();
        var topage = FormatEngine.GetLastPageNumber(req);
        ps.MaximumPage = 99999;
        ps.MinimumPage = 1;
        ps.ToPage = topage;
        ps.FromPage = 1;
        PrintToPrinter(ps, new PageSettings(ps), false);
    }

    /// <summary>
    /// Lädt die Druckereinstellungen
    /// </summary>
    /// <param name="ps">Druckereinstellungen</param>
    public void LoadPrinterData(PrinterSettings ps)
    {
        ps.PrinterName = this.PrintOptions.PrinterName;
        if (((this.SummaryInfo.ReportTitle == null) ? 0 : this.SummaryInfo.ReportTitle.Length) > 0)
        {
            ps.PrintFileName = this.SummaryInfo.ReportTitle;
        }

        switch (this.PrintOptions.PrinterDuplex)
        {
            case PrinterDuplex.Default:
                ps.Duplex = Duplex.Default;
                break;
            case PrinterDuplex.Simplex:
                ps.Duplex = Duplex.Simplex;
                break;
            case PrinterDuplex.Horizontal:
                ps.Duplex = Duplex.Horizontal;
                break;
            case PrinterDuplex.Vertical:
                ps.Duplex = Duplex.Vertical;
                break;
        }

        //Papersource laden
        var paperSource = PaperSourceKind.AutomaticFeed;
        switch (this.PrintOptions.PaperSource)
        {
            case PaperSource.Cassette:
                paperSource = PaperSourceKind.Cassette;
                break;
            case PaperSource.Manual:
                paperSource = PaperSourceKind.Manual;
                break;
            case PaperSource.Upper:
                paperSource = PaperSourceKind.Upper;
                break;
            case PaperSource.Lower:
                paperSource = PaperSourceKind.Lower;
                break;
            case PaperSource.Middle:
                paperSource = PaperSourceKind.Middle;
                break;
        }

        foreach (System.Drawing.Printing.PaperSource pas in ps.PaperSources)
        {
            if (pas.Kind == paperSource)
            {
                ps.DefaultPageSettings.PaperSource = pas;
                break;
            }
        }

        ps.DefaultPageSettings.Landscape = this.PrintOptions.PaperOrientation == PaperOrientation.Landscape;
        //ps.DefaultPageSettings.PaperSize = (PaperSize)this.PrintOptions.PaperSize;
    }

    /// <summary>
    /// Speichert die Druckereinstellungen
    /// </summary>
    /// <param name="ps"></param>
    public void SavePrinterData(PrinterSettings ps)
    {
        this.PrintOptions.PrinterName = ps.PrinterName;
        switch (ps.Duplex)
        {
            case Duplex.Default:
                this.PrintOptions.PrinterDuplex = PrinterDuplex.Default;
                break;
            case Duplex.Simplex:
                this.PrintOptions.PrinterDuplex = PrinterDuplex.Simplex;
                break;
            case Duplex.Horizontal:
                this.PrintOptions.PrinterDuplex = PrinterDuplex.Horizontal;
                break;
            case Duplex.Vertical:
                this.PrintOptions.PrinterDuplex = PrinterDuplex.Vertical;
                break;
        }

        //Papier Quelle speichern
        switch (ps.DefaultPageSettings.PaperSource.Kind)
        {
            case PaperSourceKind.AutomaticFeed:
                this.PrintOptions.PaperSource = PaperSource.Auto;
                break;
            case PaperSourceKind.Cassette:
                this.PrintOptions.PaperSource = PaperSource.Cassette;
                break;
            case PaperSourceKind.Manual:
                this.PrintOptions.PaperSource = PaperSource.Manual;
                break;
            case PaperSourceKind.Upper:
                this.PrintOptions.PaperSource = PaperSource.Upper;
                break;
            case PaperSourceKind.Lower:
                this.PrintOptions.PaperSource = PaperSource.Lower;
                break;
            case PaperSourceKind.Middle:
                this.PrintOptions.PaperSource = PaperSource.Middle;
                break;
            default:
                this.PrintOptions.PaperSource = PaperSource.Auto;
                break;
        }

        this.PrintOptions.PaperOrientation = ps.DefaultPageSettings.Landscape ? PaperOrientation.Landscape : PaperOrientation.Portrait;
        //this.PrintOptions.PaperSize = (CrystalDecisions.Shared.PaperSize)ps.DefaultPageSettings.PaperSize;
    }
}