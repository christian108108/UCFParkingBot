namespace UCFParkingBot.Library
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    
    public partial class Building
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public object Description { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("abbreviation")]
        public string Abbreviation { get; set; }

        [JsonProperty("googlemap_point")]
        public double[] Coordinates { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        public Building(string name, double[] coordinates, string abbreviation = null)
        {
            this.Name = name;
            this.Coordinates = coordinates;
            if(!string.IsNullOrWhiteSpace(abbreviation))
            {
                this.Abbreviation = abbreviation;
            }
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