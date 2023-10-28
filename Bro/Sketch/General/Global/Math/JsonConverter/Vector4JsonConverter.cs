using Bro.Json;
using System;

namespace Bro.Sketch
{
    public class Vector4JsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vector = (UnityEngine.Vector4)value;
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(vector.x);
            writer.WritePropertyName("y");
            writer.WriteValue(vector.y);
            writer.WritePropertyName("z");
            writer.WriteValue(vector.z);
            writer.WritePropertyName("w");
            writer.WriteValue(vector.w);
            writer.WriteEndObject();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.Vector4);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = Activator.CreateInstance(objectType);
            var fields = objectType.GetFields();

            while (reader.Read() && reader.Value != null)
            {
                ConverterHelper.SetField(obj, reader, fields);
            }

            return obj;
        }
    }
}
