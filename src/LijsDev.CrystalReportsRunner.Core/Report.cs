namespace LijsDev.CrystalReportsRunner.Core;

using System.Data;
using Newtonsoft.Json;

/// <summary>
/// Report
/// </summary>
[Serializable]
public sealed class Report
{
    /// <summary>
    /// Constructor with default window title
    /// </summary>
    /// <param name="filename"></param>
    public Report(string filename)
    {
        Filename = filename;
    }

    /// <summary>
    /// Constructor with custom window title
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="title"></param>
    public Report(string filename, string title)
    {
        Filename = filename;
        Title = title;
    }

    /// <summary>
    /// Constructor that allows to specify the where-statement as well as the parameters at instantiation.
    /// All parameters will be added to the internal dictionary.
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="where"></param>
    /// <param name="parameters"></param>
    public Report(string filename, string where, IEnumerable<KeyValuePair<string, object>> parameters)
    {
        Filename = filename;
        WhereStatement = where;
        foreach (var parameter in parameters)
        {
            Parameters.Add(parameter.Key, parameter.Value);
        }
    }

    /// <summary>
    /// Constructor for serialization
    /// </summary>
    /// <param name="filename"></param>
    /// <param name="title"></param>
    /// <param name="exportFilename"></param>
    /// <param name="guid"></param>
    [JsonConstructor]
    public Report(string filename, string title, string exportFilename, Guid guid)
    {
        Filename = filename;
        Title = title;
        ExportFilename = exportFilename;
        Guid = guid;
    }

    /// <summary>
    /// This Guid is used to identify the callback associated with this Report.
    /// </summary>
    public Guid Guid { get; } = Guid.NewGuid();

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

    /// <summary>
    /// Gets or sets the paper orientation for displaying the report (e.g., Portrait or Landscape).
    /// </summary>
    public PaperOrientations PaperOrientation { get; set; }

    /// <summary>
    /// Holds the database connection properties.
    /// </summary>
    public CrystalReportsConnection? Connection { get; set; }

    /// <summary>
    /// All report parameters, that the report needs.
    /// One can provide more parameters, than the report needs. The unused ones will be discarded.
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; } = [];

    /// <summary>
    /// String that can be used to extend the RecordSelectionFormula with an additional Filter
    /// </summary>
    public string WhereStatement { get; set; } = string.Empty;

    /// <summary>
    /// Used for the sample projects.
    /// </summary>
    public List<DataSet> DataSets { get; set; } = [];
}
