namespace LijsDev.CrystalReportsRunner;

using CrystalDecisions.Shared;

using LijsDev.CrystalReportsRunner.Core;
using LijsDev.CrystalReportsRunner.Shell;

internal class ReportViewer : IReportViewer
{
    public System.Windows.Forms.Form GetViewerForm(Report report, ReportViewerSettings viewerSettings)
    {
        // Fix Crystal Report param dialog with culture English (World)
        if (Thread.CurrentThread.CurrentCulture.IetfLanguageTag == "en-001")
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
        }

        // Fix Crystal Report param dialog with culture gsw-CH
        if (Thread.CurrentThread.CurrentCulture.IetfLanguageTag == "gsw-CH")
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("de-CH");
        }

        var document = ReportUtils.CreateReportDocument(report);

        // Print options
        document.PrintOptions.PaperOrientation = (PaperOrientation)report.PaperOrientation;
        if (report.PaperOrientation is PaperOrientations.Landscape)
        {
            document.PrintOptions.DissociatePageSizeAndPrinterPaperSize = false;
        }

        return new ViewerForm(document, viewerSettings)
        {
            Text = report.Title
        };
    }

    public System.Windows.Window GetViewerWindow(Report report, ReportViewerSettings settings)
    {
        // NB: We don't have dlls for WPF viewer in 13.0.16 version
        throw new NotImplementedException();
    }
}
