namespace LijsDev.CrystalReportsRunner;

using CrystalDecisions.Shared;

using LijsDev.CrystalReportsRunner.Core;
using LijsDev.CrystalReportsRunner.Shell;

internal class ReportExporter : IReportExporter
{
    /// <inheritdoc/>
    public void Print(Report report)
    {
        var document = ReportUtils.CreateReportDocument(report);

        // Print
        document.PrintToPrinter(nCopies: 1, collated: false, startPageN: 0, endPageN: 0);
    }
    /// <inheritdoc/>
    public void PrintToPrinter(Report report, string printer)
    {
        var document = ReportUtils.CreateReportDocument(report);
        document.PrintOptions.PrinterName = printer;
        // Print
        document.PrintToPrinter(nCopies: 1, collated: false, startPageN: 0, endPageN: 0);
    }

    /// <inheritdoc/>
    public void Export(Report report, ReportExportFormats exportFormat, string destinationFilename, bool overwrite = true)
    {
        var document = ReportUtils.CreateReportDocument(report);

        // Overwrite
        if (overwrite && File.Exists(destinationFilename)) File.Delete(destinationFilename);

        // Export
        document.ExportToDisk((ExportFormatType)exportFormat, destinationFilename);
    }

    /// <inheritdoc/>
    public string ExportToMemoryMappedFile(Report report, ReportExportFormats exportFormat)
    {
        var document = ReportUtils.CreateReportDocument(report);

        // Export
        var reportStream = document.ExportToStream((ExportFormatType)exportFormat);

        // Create MemoryMappedFile from Stream
        var mmfName = $"CrystalReportsRunner_Export_{Guid.NewGuid()}";
        MemoryMappedFileUtils.CreateFromStream(mmfName, reportStream);

        return mmfName;
    }
}
