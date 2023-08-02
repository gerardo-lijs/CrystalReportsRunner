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
}
