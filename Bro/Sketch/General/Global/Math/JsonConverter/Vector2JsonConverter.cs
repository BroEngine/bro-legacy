using Bro.Json;
using System;
using UnityEngine;

namespace Bro.Sketch
{
    public class Vector2JsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vector = (UnityEngine.Vector2)value;
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(vector.x);
            writer.WritePropertyName("y");
            writer.WriteValue(vector.y);
            writer.WriteEndObject();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.Vector2);
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
