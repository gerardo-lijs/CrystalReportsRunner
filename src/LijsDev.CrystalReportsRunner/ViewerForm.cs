namespace LijsDev.CrystalReportsRunner;

using System.Windows.Forms;

using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Windows.Forms;

using LijsDev.CrystalReportsRunner.Core;

internal partial class ViewerForm : Form
{
    public ViewerForm(ReportDocument document, ReportViewerSettings viewerSettings)
    {
        InitializeComponent();

        crystalReportViewer1.ReportSource = document;
        Text = document.SummaryInfo.ReportTitle;

        if (viewerSettings.UICultureLCID is not null)
        {
            crystalReportViewer1.SetProductLocale(viewerSettings.UICultureLCID.Value);
        }

        // TODO: Configure Icon
        // TODO: Configure MinimumSize
        // TODO: Configure WindowState

        crystalReportViewer1.AllowedExportFormats = (int)viewerSettings.AllowedExportFormats;

        crystalReportViewer1.ShowRefreshButton = viewerSettings.ShowRefreshButton;
        crystalReportViewer1.ShowCopyButton = viewerSettings.ShowCopyButton;
        crystalReportViewer1.ShowGroupTreeButton = viewerSettings.ShowGroupTreeButton;
        crystalReportViewer1.ShowParameterPanelButton = viewerSettings.ShowParameterPanelButton;
        crystalReportViewer1.ToolPanelView = (ToolPanelViewType)(int)viewerSettings.ToolPanelView;      // TODO: Check with Muhammad this code might point to some serialization issue

        crystalReportViewer1.EnableDrillDown = viewerSettings.EnableDrillDown;
        crystalReportViewer1.EnableRefresh = viewerSettings.EnableRefresh;

        crystalReportViewer1.ShowCloseButton = viewerSettings.ShowCloseButton;
        crystalReportViewer1.ShowPrintButton = viewerSettings.ShowPrintButton;
        crystalReportViewer1.ShowExportButton = viewerSettings.ShowExportButton;
    }
}
