namespace LijsDev.CrystalReportsRunner;

using System.IO;
using Core;
using CrystalDecisions.Shared;
using Shell;

internal class ReportExporter : IReportExporter
{
    /// <inheritdoc/>
    public void Print(Report report, ReportPrintOptions printOptions)
    {
        var document = ReportUtils.CreateReportDocument(report);
        document.Print();
    }

    /// <inheritdoc/>
    public void Export(Report report, ReportExportOptions reportExportOptions)
    {
        var document = ReportUtils.CreateReportDocument(report);

        // Print options
        document.PrintOptions.PaperOrientation = (PaperOrientation)reportExportOptions.PaperOrientation;

        // Overwrite
        if (reportExportOptions.Overwrite && File.Exists(reportExportOptions.DestinationFilename)) File.Delete(reportExportOptions.DestinationFilename);

        // Export
        document.ExportToDisk((ExportFormatType)reportExportOptions.ExportFormat, reportExportOptions.DestinationFilename);
    }

    /// <inheritdoc/>
    public string ExportToMemoryMappedFile(Report report, ReportExportToMemoryMappedFileOptions reportExportToMemoryMappedFileOptions)
    {
        var document = ReportUtils.CreateReportDocument(report);

        // Print options
        document.PrintOptions.PaperOrientation = (PaperOrientation)reportExportToMemoryMappedFileOptions.PaperOrientation;

        // Export
        var reportStream = document.ExportToStream((ExportFormatType)reportExportToMemoryMappedFileOptions.ExportFormat);

        // Create MemoryMappedFile from Stream
        var mmfName = $"CrystalReportsRunner_Export_{Guid.NewGuid()}";
        MemoryMappedFileUtils.CreateFromStream(mmfName, reportStream);

        return mmfName;
    }
}
