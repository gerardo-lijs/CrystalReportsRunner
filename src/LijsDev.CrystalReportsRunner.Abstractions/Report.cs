namespace LijsDev.CrystalReportsRunner.Abstractions;

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;

public class Report
{
    public Report(string path) : this(path, "Report")
    {

    }

    [JsonConstructor]
    public Report(string path, string title)
    {
        Path = path;
        Title = title;
    }

    public string Path { get; set; }
    public string Title { get; set; }
    public Dictionary<string, object> Parameters { get; set; } = new();
    public List<DataSet> DataSets { get; set; } = new();
}
