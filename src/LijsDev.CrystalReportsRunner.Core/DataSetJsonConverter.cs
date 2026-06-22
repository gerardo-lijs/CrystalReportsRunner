namespace LijsDev.CrystalReportsRunner.Core;

using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// NB: From testing, Newtonsoft.Json seems to serialize/deserialize DataSet and DataTable correctly, except for byte[] columns.
// DataTableJsonConverter and DataSetJsonConverter fix this issue https://github.com/gerardo-lijs/CrystalReportsRunner/issues/12

internal class JsonDataTable
{
    public string TableName { get; set; } = null!;
    public JsonDataColumn[] Columns { get; set; } = null!;
    public object[][] Rows { get; set; } = null!;
}

internal class JsonDataColumn
{
    public string ColumnName { get; set; } = null!;
    public string DataType { get; set; } = null!;
    public int MaxLength { get; set; }
}

internal class DataTableJsonConverter : JsonConverter<DataTable>
{
    private static readonly IReadOnlyDictionary<string, Type> AllowedDataTypes = CreateAllowedDataTypes();

    public override DataTable? ReadJson(JsonReader reader, Type objectType, DataTable? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var jObject = JObject.Load(reader);
        var dt = new DataTable();

        foreach (var token in jObject.Values())
        {
            if (token.Path == "TableName")
            {
                dt.TableName = token.Value<string>();
            }
            else if (token.Path == "Columns")
            {
                var jsonDataColumns = token.ToObject<JsonDataColumn[]>(serializer);
                if (jsonDataColumns is not null)
                {
                    foreach (var jsonDataColumn in jsonDataColumns)
                    {
                        var dataType = ResolveAllowedDataType(jsonDataColumn.DataType);
                        var dataColumn = dt.Columns.Add(jsonDataColumn.ColumnName, dataType);
                        if (jsonDataColumn.MaxLength > 0)
                            dataColumn.MaxLength = jsonDataColumn.MaxLength;
                    }
                }
            }
            else if (token.Path == "Rows")
            {
                var jsonDataRows = token.ToObject<object[][]>(serializer);
                if (jsonDataRows is not null)
                {
                    foreach (var jsonDataRow in jsonDataRows)
                    {
                        var drNew = dt.NewRow();
                        var i = -1;
                        foreach (DataColumn dc in dt.Columns)
                        {
                            i++;
                            if (dc.DataType == typeof(byte[]))
                            {
                                // NB: From testing, Newtonsoft.Json seems to serialize/deserialize DataSet and DataTable correctly, except for byte[] columns.
                                if (jsonDataRow[i] is null)
                                    drNew[dc] = DBNull.Value;
                                else
                                    drNew[dc] = Convert.FromBase64String(jsonDataRow[i] as string);
                            }
                            else
                            {
                                drNew[dc] = jsonDataRow[i] ?? DBNull.Value;
                            }
                        }
                        dt.Rows.Add(drNew);
                    }
                }
            }
        }
        return dt;
    }

    private static Type ResolveAllowedDataType(string? typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName))
            throw new JsonSerializationException("Missing DataColumn.DataType value.");

        if (AllowedDataTypes.TryGetValue(typeName!, out var dataType))
            return dataType;

        throw new JsonSerializationException($"Unsupported DataColumn type '{typeName}'.");
    }

    private static IReadOnlyDictionary<string, Type> CreateAllowedDataTypes()
    {
        var allowedTypes = new[]
        {
            typeof(bool),
            typeof(byte),
            typeof(byte[]),
            typeof(char),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(decimal),
            typeof(double),
            typeof(Guid),
            typeof(short),
            typeof(int),
            typeof(long),
            typeof(sbyte),
            typeof(float),
            typeof(string),
            typeof(TimeSpan),
            typeof(ushort),
            typeof(uint),
            typeof(ulong)
        };

        var supportedTypes = new Dictionary<string, Type>(StringComparer.Ordinal);
        foreach (var allowedType in allowedTypes)
        {
            supportedTypes[allowedType.ToString()] = allowedType;
            if (allowedType.FullName is not null)
                supportedTypes[allowedType.FullName] = allowedType;
            if (allowedType.AssemblyQualifiedName is not null)
                supportedTypes[allowedType.AssemblyQualifiedName] = allowedType;
        }

        return supportedTypes;
    }

    public override void WriteJson(JsonWriter writer, DataTable? value, JsonSerializer serializer)
    {
        if (value is null) return;

        var j = new JsonDataTable
        {
            TableName = value.TableName,
            Columns = value.Columns.Cast<DataColumn>().Select(c => new JsonDataColumn { ColumnName = c.ColumnName, DataType = c.DataType.ToString(), MaxLength = c.MaxLength }).ToArray(),
            Rows = value.Rows.Cast<DataRow>().Select(dr => dr.ItemArray).ToArray()
        };
        serializer.Serialize(writer, j);
    }
}

internal class DataSetJsonConverter : JsonConverter<DataSet>
{
    public override DataSet? ReadJson(JsonReader reader, Type objectType, DataSet? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var dataTableJsonConverter = new DataTableJsonConverter();
        var ds = new DataSet();
        while (reader.Read())
        {
            if (reader.TokenType == JsonToken.EndObject)
                return ds;

            if (reader.TokenType != JsonToken.PropertyName)
                throw new JsonException();

            var propertyName = reader.Value as string;
            if (propertyName == "DataSetName")
            {
                ds.DataSetName = reader.ReadAsString();
            }
            else if (propertyName == "Tables")
            {
                reader.Read();
                if (reader.TokenType != JsonToken.StartArray)
                    throw new JsonException();
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndArray)
                        break;
                    var dt = dataTableJsonConverter.ReadJson(reader, null!, null, serializer) as DataTable;
                    if (dt is not null)
                    {
                        ds.Tables.Add(dt);
                    }
                }
            }
        }
        throw new JsonException();
    }

    public override void WriteJson(JsonWriter writer, DataSet? value, JsonSerializer serializer)
    {
        if (value is null) return;

        var dataTableJsonConverter = new DataTableJsonConverter();

        writer.WriteStartObject();
        writer.WritePropertyName("DataSetName");
        writer.WriteValue(value.DataSetName);
        writer.WritePropertyName("Tables");
        writer.WriteStartArray();
        foreach (DataTable dt in value.Tables)
        {
            dataTableJsonConverter.WriteJson(writer, dt, serializer);
        }
        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}
