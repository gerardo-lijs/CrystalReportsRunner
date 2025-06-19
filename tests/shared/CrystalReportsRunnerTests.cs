namespace LijsDev.CrystalReportsRunner.UnitTests;

using CrystalDecisions.Shared;
using FluentAssertions;
using LijsDev.CrystalReportsRunner.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class CrystalReportsTests
{
    [TestMethod]
    public void SampleReport_ShowDialog_ShouldWork()
    {
        var report = new Report("SampleReport.rpt", "Sample Report")
        {
            Connection = CrystalReportsConnectionFactory.CreateSqlConnection(".\\SQLEXPRESS", "CrystalReportsSample")
        };
        report.Parameters.Add("ReportFrom", new DateTime(2022, 01, 01));
        report.Parameters.Add("UserName", "Gerardo");

        var reportViewer = new LijsDev.CrystalReportsRunner.ReportViewer();
        var form = reportViewer.GetViewerForm(report, new ReportViewerSettings());
        form.ShowDialog();
    }

    [TestMethod]
    public void SampleReport_ShowDialog_ShouldWork_ODBCSqlConnection()
    {
        var report = new Report("SampleReport.rpt", "Sample Report")
        {
            // NB: For this Unit Test to work you need to create a System DSN in ODBC Data Source Administrator configured with ODBC Driver 18 for SQL Server with the specified name and configure the database location there.
            Connection = CrystalReportsConnectionFactory.CreateODBCSqlConnection("RunnerTestDSN", "CrystalReportsSample")
        };

        report.Parameters.Add("ReportFrom", new DateTime(2022, 01, 01));
        report.Parameters.Add("UserName", "Gerardo");

        var reportViewer = new LijsDev.CrystalReportsRunner.ReportViewer();
        var form = reportViewer.GetViewerForm(report, new ReportViewerSettings());
        form.ShowDialog();
    }

    [TestMethod]
    public void SampleReport_ShowDialog_ShouldWork_ODBCSqlConnectionRegistry()
    {
        // NB: This Unit Test will automatically create a User DSN in Windows Registry at runtime.
        var dsnName = "CrystalReportRunnerDSN";
        ODBCHelper.UserDSN_SqlConnectionODBC_v17_Create(dsnName, "(local)\\SQLEXPRESS", useIntegratedSecurity: true, encrypt: true, trustServerCertificate: true);

        var report = new Report("SampleReport.rpt", "Sample Report")
        {
            Connection = CrystalReportsConnectionFactory.CreateODBCSqlConnection(dsnName, "CrystalReportsSample")
        };

        report.Parameters.Add("ReportFrom", new DateTime(2022, 01, 01));
        report.Parameters.Add("UserName", "Gerardo");

        var reportViewer = new LijsDev.CrystalReportsRunner.ReportViewer();
        var form = reportViewer.GetViewerForm(report, new ReportViewerSettings());
        form.ShowDialog();
    }

    [TestMethod]
    public void SampleReport_AskParameters_ShowDialog_ShouldWork()
    {
        var report = new Report("SampleReport.rpt", "Sample Report")
        {
            Connection = CrystalReportsConnectionFactory.CreateSqlConnection(".\\SQLEXPRESS", "CrystalReportsSample")
        };

        var reportViewer = new LijsDev.CrystalReportsRunner.ReportViewer();
        var form = reportViewer.GetViewerForm(report, new ReportViewerSettings());
        form.ShowDialog();
    }

    [TestMethod]
    public void SampleReportTwoDataSources_ShowDialog_ShouldWork()
    {
        var report = new Report("SampleReportTwoDataSources.rpt", "Sample Report with two datasources")
        {
            Connection = CrystalReportsConnectionFactory.CreateSqlConnection(".\\SQLEXPRESS", "CrystalReportsSample")
        };
        report.Parameters.Add("ReportFrom", new DateTime(2022, 01, 01));
        report.Parameters.Add("UserName", "Gerardo");

        var reportViewer = new LijsDev.CrystalReportsRunner.ReportViewer();
        var form = reportViewer.GetViewerForm(report, new ReportViewerSettings());
        form.ShowDialog();
    }

    /// <summary>
    /// Test a simple sample report without database connection, sending the DataSet with int/string/byte[] fields.
    /// </summary>
    [TestMethod]
    public void SampleReportDataset_ShowDialog_ShouldWork()
    {
        var report = new Report("SampleReportDataset.rpt", "Sample Report Dataset");
        report.Parameters.Add("ReportFrom", new DateTime(2022, 01, 01));
        report.Parameters.Add("UserName", "Gerardo");

        // Create dataset
        var sampleReportDataset = new System.Data.DataSet();

        // Create table
        var personsTable = new System.Data.DataTable("Persons");
        sampleReportDataset.Tables.Add(personsTable);
        personsTable.Columns.Add("Id", typeof(int));
        personsTable.Columns.Add("Name", typeof(string));
        personsTable.Columns.Add("Age", typeof(int));
        personsTable.Columns.Add("PersonImage", typeof(byte[]));

        // Add rows
        personsTable.Rows.Add(1, "Gerardo", "42", File.ReadAllBytes("sampleImage1.jpg"));
        personsTable.Rows.Add(2, "Khalifa", "24", File.ReadAllBytes("sampleImage2.jpg"));

        report.DataSets.Add(sampleReportDataset);

        var reportViewer = new LijsDev.CrystalReportsRunner.ReportViewer();
        var form = reportViewer.GetViewerForm(report, new ReportViewerSettings());
        form.ShowDialog();
    }

    /// <summary>
    /// Test a simple sample landscape report without database connection, sending the DataSet with int/string/byte[] fields.
    /// </summary>
    [TestMethod]
    public void SampleReportDataset_ShowDialog_WithLandscape_ShouldWork()
    {
        var report = new Report("SampleReportDataset.rpt", "Sample Report Dataset");
        report.Parameters.Add("ReportFrom", new DateTime(2022, 01, 01));
        report.Parameters.Add("UserName", "Gerardo");
        // Set landcape orientation
        report.PaperOrientation = PaperOrientations.Landscape;

        // Create dataset
        var sampleReportDataset = new System.Data.DataSet();

        // Create table
        var personsTable = new System.Data.DataTable("Persons");
        sampleReportDataset.Tables.Add(personsTable);
        personsTable.Columns.Add("Id", typeof(int));
        personsTable.Columns.Add("Name", typeof(string));
        personsTable.Columns.Add("Age", typeof(int));
        personsTable.Columns.Add("PersonImage", typeof(byte[]));

        // Add rows
        personsTable.Rows.Add(1, "Gerardo", "42", File.ReadAllBytes("sampleImage1.jpg"));
        personsTable.Rows.Add(2, "Khalifa", "24", File.ReadAllBytes("sampleImage2.jpg"));

        report.DataSets.Add(sampleReportDataset);

        var reportViewer = new LijsDev.CrystalReportsRunner.ReportViewer();
        var form = reportViewer.GetViewerForm(report, new ReportViewerSettings());
        form.ShowDialog();
    }


    [TestMethod]
    public void SampleReport_CreateReportDocument_ShouldWork()
    {
        var report = new Report("SampleReport.rpt", "Sample Report")
        {
            Connection = CrystalReportsConnectionFactory.CreateSqlConnection(".\\SQLEXPRESS", "CrystalReportsSample")
        };
        report.Parameters.Add("ReportFrom", new DateTime(2022, 01, 01));
        report.Parameters.Add("UserName", "Gerardo");

        var document = LijsDev.CrystalReportsRunner.ReportUtils.CreateReportDocument(report);

        document.Should().NotBeNull();
        document.ParameterFields.Count.Should().Be(2);
        foreach (ParameterField parameter in document.ParameterFields)
        {
            parameter.CurrentValues.Count.Should().Be(1);
        }
    }

    /// <summary>
    /// Test a simple sample report without database connection, sending the DataSet with int/string/byte[] fields and parameter with multiple values
    /// </summary>
    [TestMethod]
    public void SampleReportDatasetParameter_ShowDialog_ShouldWork()
    {
        var idListArray = new List<int> { 1, 3, 7 };

        var report = new Report("SampleReportDatasetParameters.rpt", "Sample Report Dataset Parameters");
        report.Parameters.Add("ReportFrom", new DateTime(2022, 01, 01));
        report.Parameters.Add("UserName", "Gerardo");
        report.Parameters.Add("IdList", idListArray);

        // Create dataset
        var sampleReportDataset = new System.Data.DataSet();

        // Create table
        var personsTable = new System.Data.DataTable("Persons");
        sampleReportDataset.Tables.Add(personsTable);
        personsTable.Columns.Add("Id", typeof(int));
        personsTable.Columns.Add("Name", typeof(string));
        personsTable.Columns.Add("Age", typeof(int));
        personsTable.Columns.Add("PersonImage", typeof(byte[]));

        // Add rows
        personsTable.Rows.Add(1, "Gerardo", "42", File.ReadAllBytes("sampleImage1.jpg"));
        personsTable.Rows.Add(2, "Khalifa", "24", File.ReadAllBytes("sampleImage2.jpg"));

        report.DataSets.Add(sampleReportDataset);

        var reportViewer = new LijsDev.CrystalReportsRunner.ReportViewer();
        var form = reportViewer.GetViewerForm(report, new ReportViewerSettings());
        form.ShowDialog();
    }
}
