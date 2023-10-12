namespace LijsDev.CrystalReportsRunner.UnitTests;

using FluentAssertions;
using LijsDev.CrystalReportsRunner.Core;
using LijsDev.CrystalReportsRunner.Core.UnitTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class JsonNetPipeSerializerTests
{
    [TestMethod]
    public void IntPtrJsonConverter_ShouldWork()
    {
        var windowHandle = new WindowHandle(new IntPtr(123456789));

        // Serialize
        var pipeSerializer = new JsonNetPipeSerializer();
        var serializedData = pipeSerializer.Serialize(windowHandle);

        // Deserialize
        var deserializeData = pipeSerializer.Deserialize(serializedData, windowHandle.GetType()) as WindowHandle ?? throw new InvalidCastException();

        deserializeData.Handle.Should().Be(windowHandle.Handle);
    }

    [TestMethod]
    public void ReportByteArray_Serialize_Deserialize_ShouldWork()
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
        personsTable.Rows.Add(1, "Gerardo 在京", "42", File.ReadAllBytes("sampleImage1.jpg"));
        personsTable.Rows.Add(2, "Khalifa", "24", File.ReadAllBytes("sampleImage2.jpg"));

        //report.DataSets.Add(sampleReportDataset);

        // Serialize
        var pipeSerializer = new JsonNetPipeSerializer();
        var serializedData = pipeSerializer.Serialize(report);

        // Deserialize
        var deserializeData = pipeSerializer.Deserialize(serializedData, report.GetType()) as Report ?? throw new InvalidCastException();

        //// Assert data type int
        //var deserialize_Row0_Id = deserializeData.DataSets[0].Tables[0].Rows[0]["Id"];
        //var original_Row0_Id = report.DataSets[0].Tables[0].Rows[0]["Id"];
        //deserialize_Row0_Id.Should().Be(original_Row0_Id);

        //var deserialize_Row1_Id = deserializeData.DataSets[0].Tables[0].Rows[1]["Id"];
        //var original_Row1_Id = report.DataSets[0].Tables[0].Rows[1]["Id"];
        //deserialize_Row1_Id.Should().Be(original_Row1_Id);

        //// Assert data type string unicode
        //var deserialize_Row0_Name = deserializeData.DataSets[0].Tables[0].Rows[0]["Name"];
        //var original_Row0_Name = report.DataSets[0].Tables[0].Rows[0]["Name"];
        //deserialize_Row0_Name.Should().Be(original_Row0_Name);

        //var deserialize_Row1_Name = deserializeData.DataSets[0].Tables[0].Rows[1]["Name"];
        //var original_Row1_Name = report.DataSets[0].Tables[0].Rows[1]["Name"];
        //deserialize_Row1_Name.Should().Be(original_Row1_Name);

        //// Assert data type byte[]
        //var deserialize_Row0_PersonImage = deserializeData.DataSets[0].Tables[0].Rows[0]["PersonImage"] as byte[];
        //var original_Row0_PersonImage = report.DataSets[0].Tables[0].Rows[0]["PersonImage"] as byte[];

        //// Debug - save bytes - image is a match with original
        ////System.IO.File.WriteAllBytes("deserialize_Row0_PersonImage.jpg", deserialize_Row0_PersonImage);

        //deserialize_Row0_PersonImage!.Length.Should().Be(original_Row0_PersonImage!.Length);

        //var deserialize_Row1_PersonImage = deserializeData.DataSets[0].Tables[0].Rows[1]["PersonImage"] as byte[];
        //var original_Row1_PersonImage = report.DataSets[0].Tables[0].Rows[1]["PersonImage"] as byte[];

        //// Debug - save bytes - image is a match with original
        //System.IO.File.WriteAllBytes("deserialize_Row1_PersonImage.jpg", deserialize_Row1_PersonImage);

        //deserialize_Row1_PersonImage!.Length.Should().Be(original_Row1_PersonImage!.Length);
    }

    [TestMethod]
    public void TestByteArray_Serialize_Deserialize_ShouldWork()
    {
        var report = new SerializeTestClass();

        report.TestImage = File.ReadAllBytes("sampleImage1.jpg");

        ////// Create dataset
        ////var sampleReportDataset = new System.Data.DataSet();

        //// Create table
        //var personsTable = new System.Data.DataTable("Persons");
        ////sampleReportDataset.Tables.Add(personsTable);
        //personsTable.Columns.Add("Id", typeof(int));
        //personsTable.Columns.Add("Name", typeof(string));
        //personsTable.Columns.Add("Age", typeof(int));
        //personsTable.Columns.Add("PersonImage", typeof(byte[]));

        //// Add rows
        //personsTable.Rows.Add(1, "Gerardo 在京", "42", File.ReadAllBytes("sampleImage1.jpg"));
        //personsTable.Rows.Add(2, "Khalifa", "24", File.ReadAllBytes("sampleImage2.jpg"));

        ////report.DataSets.Add(sampleReportDataset);
        //report.TestDataTable = personsTable;

        // Serialize
        var pipeSerializer = new JsonNetPipeSerializer();
        var serializedData = pipeSerializer.Serialize(report);

        // Deserialize
        var deserializeData = pipeSerializer.Deserialize(serializedData, report.GetType()) as SerializeTestClass ?? throw new InvalidCastException();

        // Asert test image
        System.IO.File.WriteAllBytes("deserialize_TestImage.jpg", deserializeData.TestImage);

        //// Assert data type int
        //var deserialize_Row0_Id = deserializeData.DataSets[0].Tables[0].Rows[0]["Id"];
        //var original_Row0_Id = report.DataSets[0].Tables[0].Rows[0]["Id"];
        //deserialize_Row0_Id.Should().Be(original_Row0_Id);

        //var deserialize_Row1_Id = deserializeData.DataSets[0].Tables[0].Rows[1]["Id"];
        //var original_Row1_Id = report.DataSets[0].Tables[0].Rows[1]["Id"];
        //deserialize_Row1_Id.Should().Be(original_Row1_Id);

        //// Assert data type string unicode
        //var deserialize_Row0_Name = deserializeData.DataSets[0].Tables[0].Rows[0]["Name"];
        //var original_Row0_Name = report.DataSets[0].Tables[0].Rows[0]["Name"];
        //deserialize_Row0_Name.Should().Be(original_Row0_Name);

        //var deserialize_Row1_Name = deserializeData.DataSets[0].Tables[0].Rows[1]["Name"];
        //var original_Row1_Name = report.DataSets[0].Tables[0].Rows[1]["Name"];
        //deserialize_Row1_Name.Should().Be(original_Row1_Name);

        //// Assert data type byte[]
        //var deserialize_Row0_PersonImage = deserializeData.DataSets[0].Tables[0].Rows[0]["PersonImage"] as long[];
        //var original_Row0_PersonImage = report.DataSets[0].Tables[0].Rows[0]["PersonImage"] as byte[];

        //// Debug - save bytes - image is a match with original
        ////System.IO.File.WriteAllBytes("deserialize_Row0_PersonImage.jpg", deserialize_Row0_PersonImage);

        //deserialize_Row0_PersonImage!.Length.Should().Be(original_Row0_PersonImage!.Length);

        //var deserialize_Row1_PersonImage = deserializeData.DataSets[0].Tables[0].Rows[1]["PersonImage"] as byte[];
        //var original_Row1_PersonImage = report.DataSets[0].Tables[0].Rows[1]["PersonImage"] as byte[];

        //// Debug - save bytes - image is a match with original
        //System.IO.File.WriteAllBytes("deserialize_Row1_PersonImage.jpg", deserialize_Row1_PersonImage);

        //deserialize_Row1_PersonImage!.Length.Should().Be(original_Row1_PersonImage!.Length);
    }
}
