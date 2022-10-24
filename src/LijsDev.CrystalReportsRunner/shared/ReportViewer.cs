namespace LijsDev.CrystalReportsRunner;

using System.Windows.Forms;

using LijsDev.CrystalReportsRunner.Core;
using LijsDev.CrystalReportsRunner.Shell;

internal class ReportViewer : IReportViewer
{
    public Form GetViewerForm(Report report, ReportViewerSettings viewerSettings)
    {
        var document = ReportUtils.CreateReportDocument(report);
        return new ViewerForm(document, viewerSettings)
        {
            Text = report.Title
        };
    }
}
