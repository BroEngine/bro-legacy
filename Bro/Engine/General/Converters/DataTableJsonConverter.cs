using System;
using Bro.Json;

namespace Bro
{
    public class DataTableJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var table = value as DataTable;
            var base64 = DataTableConverter.ToBase64(table);

            writer.WriteValue(base64);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            var table = new DataTable();

            if (reader.TokenType == JsonToken.String)
            {
                table = DataTableConverter.FromBase64(reader.Value as string);
            }

            reader.Read();
            reader.Read();

            return table;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DataTable);
        }
    }
}