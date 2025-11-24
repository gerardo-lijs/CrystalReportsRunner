namespace LijsDev.CrystalReportsRunner.Core.UnitTests;

using FluentAssertions;
using Xunit;

public class DataSetJsonConverterTests
{
    [Fact]
    public void DataSet_Serialize_Deserialize_ShouldWork()
    {
        // Create dataset
        var sampleReportDataset = new System.Data.DataSet
        {
            DataSetName = "TestName"
        };

        // Create table
        var personsTable = new System.Data.DataTable("Persons");
        sampleReportDataset.Tables.Add(personsTable);
        personsTable.Columns.Add("Id", typeof(int));
        personsTable.Columns.Add("Name", typeof(string));
        personsTable.Columns.Add("Age", typeof(int));
        personsTable.Columns.Add("PersonImage", typeof(byte[]));
        personsTable.Columns.Add("TestBool", typeof(bool));
        personsTable.Columns.Add("TestInt64", typeof(long));
        personsTable.Columns.Add("TestDateTime", typeof(DateTime));
        personsTable.Columns.Add("TestDouble", typeof(double));
        personsTable.Columns.Add("TestGuid", typeof(Guid));
        personsTable.Columns.Add("TestDBNull", typeof(int));

        // Add rows
        personsTable.Rows.Add(1, "Gerardo 在京", "42", File.ReadAllBytes("sampleImage1.jpg"), true, long.MaxValue, new DateTime(2023, 10, 12, 21, 27, 15), 1.23, Guid.NewGuid(), null);
        personsTable.Rows.Add(2, "Khalifa", "24", File.ReadAllBytes("sampleImage2.jpg"), false, long.MinValue, new DateTime(2023, 2, 7, 1, 2, 3), double.NaN, Guid.NewGuid(), DBNull.Value);
        personsTable.Rows.Add(3, "Albert", "22", null, false, long.MinValue, new DateTime(2023, 2, 7, 1, 2, 3), double.NaN, Guid.NewGuid(), DBNull.Value);

        // Serialize
        var pipeSerializer = new JsonNetPipeSerializer();
        var serializedData = pipeSerializer.Serialize(sampleReportDataset);

        // Deserialize
        var deserializeData = pipeSerializer.Deserialize(serializedData, sampleReportDataset.GetType()) as System.Data.DataSet ?? throw new InvalidCastException();

        // Assert data type int
        var deserialize_Row0_Id = deserializeData.Tables[0].Rows[0]["Id"];
        var original_Row0_Id = sampleReportDataset.Tables[0].Rows[0]["Id"];
        deserialize_Row0_Id.Should().Be(original_Row0_Id);

        var deserialize_Row1_Id = deserializeData.Tables[0].Rows[1]["Id"];
        var original_Row1_Id = sampleReportDataset.Tables[0].Rows[1]["Id"];
        deserialize_Row1_Id.Should().Be(original_Row1_Id);

        // Assert data type string unicode
        var deserialize_Row0_Name = deserializeData.Tables[0].Rows[0]["Name"];
        var original_Row0_Name = sampleReportDataset.Tables[0].Rows[0]["Name"];
        deserialize_Row0_Name.Should().Be(original_Row0_Name);

        var deserialize_Row1_Name = deserializeData.Tables[0].Rows[1]["Name"];
        var original_Row1_Name = sampleReportDataset.Tables[0].Rows[1]["Name"];
        deserialize_Row1_Name.Should().Be(original_Row1_Name);

        // Assert data type byte[]
        var deserialize_Row0_PersonImage = deserializeData.Tables[0].Rows[0]["PersonImage"] as byte[];
        var original_Row0_PersonImage = sampleReportDataset.Tables[0].Rows[0]["PersonImage"] as byte[];

        // Debug - save bytes - image is a match with original
        System.IO.File.WriteAllBytes("deserialize_Row0_PersonImage.jpg", deserialize_Row0_PersonImage);

        deserialize_Row0_PersonImage!.Length.Should().Be(original_Row0_PersonImage!.Length);

        var deserialize_Row1_PersonImage = deserializeData.Tables[0].Rows[1]["PersonImage"] as byte[];
        var original_Row1_PersonImage = sampleReportDataset.Tables[0].Rows[1]["PersonImage"] as byte[];

        // Debug - save bytes - image is a match with original
        System.IO.File.WriteAllBytes("deserialize_Row1_PersonImage.jpg", deserialize_Row1_PersonImage);

        deserialize_Row1_PersonImage!.Length.Should().Be(original_Row1_PersonImage!.Length);
    }
}
