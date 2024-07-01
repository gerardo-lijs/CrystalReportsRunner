namespace LijsDev.CrystalReportsRunner;
using CrystalDecisions.Shared;

using LijsDev.CrystalReportsRunner.Core;
using LijsDev.CrystalReportsRunner.Shell;

internal class ReportExporter : IReportExporter
{
    /// <inheritdoc/>
    public void Print(Report report, string? printerName)
    {
        var document = ReportUtils.CreateReportDocument(report);

        // Specified printer
        if (!string.IsNullOrWhiteSpace(printerName))
        {
            var printSettings = new System.Drawing.Printing.PrinterSettings
            {
                PrinterName = printerName,
                Copies = 1,
                Collate = false
            };

            document.PrintToPrinter(printSettings, new System.Drawing.Printing.PageSettings(), false);

        }
        else
        {

            // Print
            document.PrintToPrinter(nCopies: 1, collated: false, startPageN: 0, endPageN: 0);
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
