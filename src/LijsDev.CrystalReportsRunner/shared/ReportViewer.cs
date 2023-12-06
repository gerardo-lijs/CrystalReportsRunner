namespace LijsDev.CrystalReportsRunner;

using System.Windows.Forms;

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
        return new ViewerForm(document, viewerSettings)
        {
            Text = report.Title
        };
    }
}
