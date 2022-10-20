using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Data;

namespace LijsDev.CrystalReportsRunner.Abstractions
{
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

    public class WindowHandle
    {
        [JsonConstructor]
        public WindowHandle(IntPtr handle)
        {
            HandleInternal = handle.ToInt64();
        }

        private long HandleInternal { get; }
        public IntPtr Handle => new IntPtr(HandleInternal);
    }

    public class DbConnection
    {
        public DbConnection()
        {

        }

        /// <summary>
        /// Create a DbConnection setting from a connection string.
        /// </summary>
        /// <param name="connectionString"></param>
        public DbConnection(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Create a DbConnection that uses Integrated Security.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="database"></param>
        public DbConnection(string server, string database)
        {
            Server = server;
            Database = database;
            UseIntegratedSecurity = true;
        }

        /// <summary>
        /// Create a DbConnection that uses SQL Server authentication.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="database"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public DbConnection(string server, string database, string username, string password)
        {
            Server = server;
            Database = database;
            Username = username;
            Password = password;
            UseIntegratedSecurity = false;
        }

        public string? ConnectionString { get; set; }
        public string? Server { get; set; }
        public string? Database { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public bool UseIntegratedSecurity { get; set; }
    }

    public enum CrystalReportsViewerExportFormats
    {
        NoFormat = 0,
        PdfFormat = 1,
        ExcelFormat = 2,
        WordFormat = 4,
        RtfFormat = 8,
        RptFormat = 16,
        ExcelRecordFormat = 32,
        EditableRtfFormat = 64,
        XmlFormat = 128,
        RptrFormat = 256,
        XLSXFormat = 512,
        CsvFormat = 1024,
        XLSXPagebasedFormat = 2048,
        XLSXRecordFormat = 4096,
        AllFormats = 268435455
    }

    public enum CrystalReportsToolPanelViewType
    {
        None = 0,
        GroupTree = 1,
        ParameterPanel = 2
    }

    public class ReportViewerSettings
    {
        public int? ProductLacaleLCID { get; set; }
        public CrystalReportsViewerExportFormats AllowedExportFormats { get; set; } = CrystalReportsViewerExportFormats.AllFormats;

        public bool ShowRefreshButton { get; set; } = true;
        public bool ShowCopyButton { get; set; } = true;
        public bool ShowGroupTreeButton { get; set; } = true;
        public bool ShowParameterPanelButton { get; set; } = true;
        public CrystalReportsToolPanelViewType ToolPanelView { get; set; } = CrystalReportsToolPanelViewType.GroupTree;
        public bool EnableDrillDown { get; set; } = true;
        public bool EnableRefresh { get; set; } = true;
        public bool ShowCloseButton { get; set; } = true;
    }

}
