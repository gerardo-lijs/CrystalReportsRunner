namespace LijsDev.CrystalReportsRunner;

using System.Windows.Forms;

using CrystalDecisions.Shared;

using LijsDev.CrystalReportsRunner.Core;
using LijsDev.CrystalReportsRunner.Shell;

internal class ReportViewer : IReportViewer
{
    public Form GetViewerForm(Report report, ReportViewerSettings viewerSettings)
    {
        // Fix Crystal Report param dialog with culture English (World)
        if (Thread.CurrentThread.CurrentCulture.IetfLanguageTag == "en-001")
        {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en");
        }

        var document = ReportUtils.CreateReportDocument(report);

        // Print options
        document.PrintOptions.PaperOrientation = (PaperOrientation)report.PaperOrientation;

        // NB: We need to set this in the report options or programatically for Landscape to display correctly.
        // Leave it as it is but can be enabled for testing purposes.
        //document.PrintOptions.DissociatePageSizeAndPrinterPaperSize = false;

        return new ViewerForm(document, viewerSettings)
        {
            Text = report.Title
        };
    }
}
