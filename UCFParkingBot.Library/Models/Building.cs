namespace UCFParkingBot.Library
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types 
    
    public partial class Building : TableEntity
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public object Description { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("abbreviation")]
        public string Abbreviation { get => this.RowKey; set => this.RowKey = value; }

        [JsonProperty("googlemap_point")]
        public double[] Coordinates { get; set; }

        public double Latitude { get => this.Coordinates[0]; set => this.Coordinates[0] = value;}

        public double Longitude { get => this.Coordinates[1]; set => this.Coordinates[1] = value; }

        [JsonProperty("id")]
        public string Id { get; set; }

        public Building(string name, double[] coordinates, string abbreviation = null)
        {
            // for Azure Table Storage
            this.PartitionKey = "building";

            this.Name = name;
            this.Coordinates = coordinates;
            this.Abbreviation = abbreviation;
        }
    }
    public partial class Building
    {
        public static Building FromJson(string json) => JsonConvert.DeserializeObject<Building>(json, UCFParkingBot.Library.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Building self) => JsonConvert.SerializeObject(self, UCFParkingBot.Library.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }
    }
}