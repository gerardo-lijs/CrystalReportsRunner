namespace LijsDev.CrystalReportsRunner.Abstractions;
public interface ICrystalReportsRunner
{
    void ShowReport(
        Report report,
        ReportViewerSettings viewerSettings,
        WindowHandle? parent = null,
        DbConnection? connection = null);

    void ShowReportDialog(
        Report report,
        ReportViewerSettings viewSettings,
        WindowHandle parent,
        DbConnection? connection);
}

public interface ICrystalReportsCaller { }

public class DefaultCrystalReportsCaller : ICrystalReportsCaller { }
