namespace LijsDev.CrystalReportsRunner;

using System.Globalization;
using System.Windows;
using Core;
using CrystalDecisions.Shared;
using Shell;

internal class ReportViewer : IReportViewer
{
    public Form GetViewerForm(Report report, ReportViewerSettings viewerSettings)
    {
        // Fix Crystal Report param dialog with culture English (World)
        if (Thread.CurrentThread.CurrentCulture.IetfLanguageTag == "en-001")
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
        }

        var document = ReportUtils.CreateReportDocument(report);

        // Print options
        document.PrintOptions.PaperOrientation = (PaperOrientation)report.PaperOrientation;
        if (report.PaperOrientation == PaperOrientations.Landscape)
        {
            document.PrintOptions.DissociatePageSizeAndPrinterPaperSize = false;
        }

        return new ViewerForm(document, viewerSettings) { Text = report.Title };
    }

    public Window GetViewerWindow(Report report, ReportViewerSettings settings)
    {
        var document = ReportUtils.CreateReportDocument(report);
        var viewModel = new ReportViewerWindowVM(document, settings);
        return new ReportViewerWindow(viewModel);
    }
}
