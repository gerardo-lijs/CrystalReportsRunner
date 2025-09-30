namespace LijsDev.CrystalReportsRunner;

using System.Drawing.Printing;
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

        // Print options
        document.PrintOptions.PaperOrientation = (PaperOrientation)printOptions.PaperOrientation;

        // Specified printer
        if (!string.IsNullOrWhiteSpace(printOptions.PrinterName))
        {
            var printSettings = new PrinterSettings { PrinterName = printOptions.PrinterName, Copies = printOptions.Copies, Collate = printOptions.Collated };

            if (printOptions.StartPageNumber != 0 && printOptions.EndPageNumber != 0)
            {
                printSettings.PrintRange = PrintRange.SomePages;
                printSettings.FromPage = printOptions.StartPageNumber;
                printSettings.ToPage = printOptions.EndPageNumber;
            }

            document.PrintToPrinter(printSettings, new PageSettings(), false);
        }
        else
        {
            // Print
            document.PrintToPrinter(printOptions.Copies, printOptions.Collated, printOptions.StartPageNumber, printOptions.EndPageNumber);
        }
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
