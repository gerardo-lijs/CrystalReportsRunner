namespace LijsDev.CrystalReportsRunner;

using System.Windows;
using CrystalDecisions.CrystalReports.Engine;

public partial class ViewerWindow : Window
{
    private readonly ReportDocument _document;

    /// <inheritdoc/>
    public ViewerWindow(string reportTitle, CrystalDecisions.CrystalReports.Engine.ReportDocument document, Core.ReportViewerSettings viewerSettings)
    {
        InitializeComponent();

        // TODO: Program Icon for WPF
        //// Set Icon
        //if (viewerSettings.WindowIcon is not null)
        //{
        //    try
        //    {
        //        using var ms = new System.IO.MemoryStream(viewerSettings.WindowIcon);
        //        var iconBitmap = new Bitmap(ms);
        //        Icon = Icon.FromHandle(iconBitmap.GetHicon());
        //    }
        //    catch (Exception ex)
        //    {
        //        // TODO: Communicate to caller app the error in a better way. For now, the log will do.
        //        Logger.Fatal(ex);
        //    }
        //}

        // Fix Application.Current not initialized. This is needed for Print to work
        if (System.Windows.Application.Current is null)
        {
            new System.Windows.Application();
        }

        // Configure Form
        Title = reportTitle;
        //_closeOnEscapeKey = viewerSettings.WindowCloseOnEscapeKey;
        ShowInTaskbar = viewerSettings.WindowShowInTaskbar;
        if (viewerSettings.WindowMinimumWidth is not null && viewerSettings.WindowMinimumHeight is not null)
        {
            MinWidth = viewerSettings.WindowMinimumWidth.Value;
            MinHeight = viewerSettings.WindowMinimumHeight.Value;
        }
        if (viewerSettings.WindowMaximumWidth is not null && viewerSettings.WindowMaximumHeight is not null)
        {
            MaxWidth = viewerSettings.WindowMaximumWidth.Value;
            MaxHeight = viewerSettings.WindowMaximumHeight.Value;
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

        // ResizeMode
        if ((viewerSettings.WindowAllowMaximize && viewerSettings.WindowAllowMinimize) || viewerSettings.WindowAllowResize)
        {
            ResizeMode = System.Windows.ResizeMode.CanResize;
        }
        else if (!viewerSettings.WindowAllowMaximize && viewerSettings.WindowAllowMinimize)
        {
            ResizeMode = System.Windows.ResizeMode.CanMinimize;
        }
        else if (viewerSettings.WindowAllowMinimize)
        {
            ResizeMode = System.Windows.ResizeMode.CanMinimize;
        }
        else
        {
            ResizeMode = System.Windows.ResizeMode.NoResize;
        }

        // WindowState
        switch (viewerSettings.WindowInitialState)
        {
            case Core.ReportViewerWindowState.Normal:
                WindowState = WindowState.Normal;
                break;
            case Core.ReportViewerWindowState.Minimized:
                WindowState = WindowState.Minimized;
                break;
            case Core.ReportViewerWindowState.Maximized:
                WindowState = WindowState.Maximized;
                break;
        }
        //StartPosition = (FormStartPosition)viewerSettings.WindowInitialPosition;

        //// Resize disabled
        //if (!viewerSettings.WindowAllowResize)
        //{
        //    FormBorderStyle = FormBorderStyle.FixedSingle;
        //}

        // Configure Crystal Report Viewer
        if (!viewerSettings.ShowReportTabs)
        {
            //SetReportTabsVisible(false);
        }

        if (viewerSettings.UICultureLCID is not null)
        {
            CrystalReportsViewerControl.ViewerCore.ProductLocale = new System.Globalization.CultureInfo(viewerSettings.UICultureLCID.Value);
        }

        CrystalReportsViewerControl.ViewerCore.AllowedExportFormats = (int)viewerSettings.AllowedExportFormats;

        CrystalReportsViewerControl.ShowRefreshButton = viewerSettings.ShowRefreshButton;
        CrystalReportsViewerControl.ShowToggleSidePanelButton = viewerSettings.ShowGroupTreeButton && viewerSettings.ShowParameterPanelButton;
        CrystalReportsViewerControl.ViewerCore.ToggleSidePanel = viewerSettings.ToolPanelView switch
        {
            Core.ReportViewerToolPanelViewType.None => SAPBusinessObjects.WPF.Viewer.Constants.SidePanelKind.None,
            Core.ReportViewerToolPanelViewType.GroupTree => SAPBusinessObjects.WPF.Viewer.Constants.SidePanelKind.GroupTree,
            Core.ReportViewerToolPanelViewType.ParameterPanel => SAPBusinessObjects.WPF.Viewer.Constants.SidePanelKind.ParameterPanel,
            _ => throw new NotImplementedException(),
        };
        CrystalReportsViewerControl.ViewerCore.EnableDrillDown = viewerSettings.EnableDrillDown;
        CrystalReportsViewerControl.ViewerCore.EnableRefresh = viewerSettings.EnableRefresh;

        CrystalReportsViewerControl.ShowCopyButton = viewerSettings.ShowCopyButton;
        CrystalReportsViewerControl.ShowPrintButton = viewerSettings.ShowPrintButton;
        CrystalReportsViewerControl.ShowExportButton = viewerSettings.ShowExportButton;

        //CrystalReportsViewerControl.ShowCloseButton = viewerSettings.ShowCloseButton;
        //CrystalReportsViewerControl.ShowZoomButton = viewerSettings.ShowZoomButton;

        if (viewerSettings.ZoomLevel is not 100)
        {
            CrystalReportsViewerControl.ViewerCore.Zoom(viewerSettings.ZoomLevel);
        }

        _document = document;
    }

    private void CrystalReportsViewerControl_Loaded(object sender, RoutedEventArgs e)
    {
        // Set report
        CrystalReportsViewerControl.ViewerCore.ReportSource = _document;
    }
}
