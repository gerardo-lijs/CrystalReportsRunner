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
