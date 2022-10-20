namespace LijsDev.CrystalReportsRunner.Core;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;

/// <summary>
/// Report
/// </summary>
public class Report
{
    /// <inheritdoc/>
    public Report(string path) : this(path, "Report")
    {

    }

    /// <inheritdoc/>
    [JsonConstructor]
    public Report(string path, string title)
    {
        Path = path;
        Title = title;
    }

    /// <inheritdoc/>
    public string Path { get; set; }
    /// <inheritdoc/>
    public string Title { get; set; }
    /// <inheritdoc/>
    public Dictionary<string, object> Parameters { get; set; } = new();
    /// <inheritdoc/>
    public List<DataSet> DataSets { get; set; } = new();
}
