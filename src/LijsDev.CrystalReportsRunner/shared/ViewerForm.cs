namespace LijsDev.CrystalReportsRunner;

using System.Windows.Forms;

using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Windows.Forms;

using LijsDev.CrystalReportsRunner.Core;

internal partial class ViewerForm : Form
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private readonly bool _closeOnEscapeKey;

    public ViewerForm(ReportDocument document, ReportViewerSettings viewerSettings)
    {
        InitializeComponent();

        // Set Icon
        if (viewerSettings.WindowIcon is not null)
        {
            try
            {
                using var ms = new MemoryStream(viewerSettings.WindowIcon);
                var iconBitmap = new Bitmap(ms);
                Icon = Icon.FromHandle(iconBitmap.GetHicon());
            }
            catch (Exception ex)
            {
                // TODO: Communicate to caller app the error in a better way. For now, the log will do.
                Logger.Fatal(ex);
            }
        }

        // Configure Form
        _closeOnEscapeKey = viewerSettings.WindowCloseOnEscapeKey;
        ShowInTaskbar = viewerSettings.WindowShowInTaskbar;
        if (viewerSettings.WindowMinimumWidth is not null && viewerSettings.WindowMinimumHeight is not null)
        {
            MinimumSize = new Size(viewerSettings.WindowMinimumWidth.Value, viewerSettings.WindowMinimumHeight.Value);
        }
        if (viewerSettings.WindowMaximumWidth is not null && viewerSettings.WindowMaximumHeight is not null)
        {
            MaximumSize = new Size(viewerSettings.WindowMaximumWidth.Value, viewerSettings.WindowMaximumHeight.Value);
        }
        if (viewerSettings.WindowLocationHeight is not null)
        {
            Height = viewerSettings.WindowLocationHeight.Value;
        }
        if (viewerSettings.WindowLocationWidth is not null)
        {
            Width = viewerSettings.WindowLocationWidth.Value;
        }
        if (viewerSettings.WindowLocationLeft is not null)
        {
            Left = viewerSettings.WindowLocationLeft.Value;
        }
        if (viewerSettings.WindowLocationTop is not null)
        {
            Top = viewerSettings.WindowLocationTop.Value;
        }
        MinimizeBox = viewerSettings.WindowAllowMinimize;
        MaximizeBox = viewerSettings.WindowAllowMaximize;
        WindowState = (FormWindowState)viewerSettings.WindowInitialState;
        StartPosition = (FormStartPosition)viewerSettings.WindowInitialPosition;

        // Resize disabled
        if (!viewerSettings.WindowAllowResize)
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        // Set report
        crystalReportViewer1.ReportSource = document;

        // Configure Crystal Report Viewer
        if (!viewerSettings.ShowReportTabs)
        {
            SetReportTabsVisible(false);
        }

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

    protected override bool ProcessDialogKey(Keys keyData)
    {
        if (_closeOnEscapeKey && ModifierKeys is Keys.None && keyData is Keys.Escape)
        {
            Close();
            return true;
        }
        return base.ProcessDialogKey(keyData);
    }

    private void SetReportTabsVisible(bool visible)
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
