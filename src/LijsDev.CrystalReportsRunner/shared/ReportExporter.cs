namespace LijsDev.CrystalReportsRunner;
using CrystalDecisions.Shared;

using LijsDev.CrystalReportsRunner.Core;
using LijsDev.CrystalReportsRunner.Shell;

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
            var printSettings = new System.Drawing.Printing.PrinterSettings
            {
                PrinterName = printOptions.PrinterName,
                Copies = printOptions.Copies,
                Collate = printOptions.Collated
            };

            if (printOptions.StartPageNumber != 0 && printOptions.EndPageNumber != 0)
            {
                printSettings.PrintRange = System.Drawing.Printing.PrintRange.SomePages;
                printSettings.FromPage = printOptions.StartPageNumber;
                printSettings.ToPage = printOptions.EndPageNumber;
            }

            document.PrintToPrinter(printSettings, new System.Drawing.Printing.PageSettings(), false);
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
