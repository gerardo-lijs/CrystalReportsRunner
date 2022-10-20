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

        // Configure Form
        if (viewerSettings.WindowMinimumWidth is not null && viewerSettings.WindowMinimumHeight is not null)
        {
            MinimumSize = new Size(viewerSettings.WindowMinimumWidth.Value, viewerSettings.WindowMinimumHeight.Value);
        }
        WindowState = (FormWindowState)(int)viewerSettings.WindowInitialState;

        // Set report
        crystalReportViewer1.ReportSource = document;
        Text = document.SummaryInfo.ReportTitle;        // TODO: We are not using the specified Window title here. This is good as default

        // TODO: Review what this does exactly. Brought from legacy caller app
        //SetViewerTabsVisible(false);

        // Configure Crystal Report Viewer
        if (viewerSettings.UICultureLCID is not null)
        {
            crystalReportViewer1.SetProductLocale(viewerSettings.UICultureLCID.Value);
        }

        crystalReportViewer1.AllowedExportFormats = (int)viewerSettings.AllowedExportFormats;

        crystalReportViewer1.ShowRefreshButton = viewerSettings.ShowRefreshButton;
        crystalReportViewer1.ShowGroupTreeButton = viewerSettings.ShowGroupTreeButton;
        crystalReportViewer1.ShowParameterPanelButton = viewerSettings.ShowParameterPanelButton;
        crystalReportViewer1.ToolPanelView = (ToolPanelViewType)(int)viewerSettings.ToolPanelView;

        crystalReportViewer1.EnableDrillDown = viewerSettings.EnableDrillDown;
        crystalReportViewer1.EnableRefresh = viewerSettings.EnableRefresh;

        crystalReportViewer1.ShowCopyButton = viewerSettings.ShowCopyButton;
        crystalReportViewer1.ShowCloseButton = viewerSettings.ShowCloseButton;
        crystalReportViewer1.ShowPrintButton = viewerSettings.ShowPrintButton;
        crystalReportViewer1.ShowExportButton = viewerSettings.ShowExportButton;
        crystalReportViewer1.ShowZoomButton = viewerSettings.ShowZoomButton;
    }

    private void SetViewerTabsVisible(bool visible)
    {
        foreach (Control control in crystalReportViewer1.Controls)
        {
            if ((control is PageView view) && (control.Controls.Count > 0))
            {
                var tab = (TabControl)view.Controls[0];
                if (!visible)
                {
                    tab.ItemSize = new Size(0, 1);
                    tab.SizeMode = TabSizeMode.Fixed;
                    tab.Appearance = TabAppearance.Buttons;
                }
                else
                {
                    tab.ItemSize = new Size(67, 18);
                    tab.SizeMode = TabSizeMode.Normal;
                    tab.Appearance = TabAppearance.Normal;
                }
            }
        }
    }
}
