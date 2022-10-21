namespace LijsDev.CrystalReportsRunner.Core;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;

/// <summary>
/// Report
/// </summary>
[Serializable]
public sealed class Report
{
    /// <inheritdoc/>
    public Report(string filename)
    {
        Filename = filename;
    }

    /// <inheritdoc/>
    public Report(string filename, string title)
    {
        Filename = filename;
        Title = title;
    }

    /// <inheritdoc/>
    [JsonConstructor]
    public Report(string filename, string title, string exportFilename)
    {
        Filename = filename;
        Title = title;
        ExportFilename = exportFilename;
    }

    /// <summary>
    /// Crystal Reports RPT filename
    /// </summary>
    public string Filename { get; set; }

    /// <summary>
    /// Report title to be displayed in the Report Viewer Window title.
    /// Default: 'Report'
    /// </summary>
    public string Title { get; set; } = "Report";

    /// <summary>
    /// Initial export filename that will appear in the Save Dialog when exporting the report.
    /// Default: null (will use the report Title)
    /// </summary>
    public string? ExportFilename { get; set; }

    /// <inheritdoc/>
    public CrystalReportsConnection? Connection { get; set; }

    /// <inheritdoc/>
    public Dictionary<string, object> Parameters { get; set; } = new();

    /// <inheritdoc/>
    public List<DataSet> DataSets { get; set; } = new();
}
