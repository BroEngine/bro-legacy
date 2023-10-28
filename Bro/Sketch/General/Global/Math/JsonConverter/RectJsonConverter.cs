using Bro.Json;
using System;
using UnityEngine;

namespace Bro.Sketch
{
    public class RectJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var rect = (UnityEngine.Rect)value;
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(rect.x);
            writer.WritePropertyName("y");
            writer.WriteValue(rect.y);
            writer.WritePropertyName("width");
            writer.WriteValue(rect.width);
            writer.WritePropertyName("height");
            writer.WriteValue(rect.height);
            writer.WriteEndObject();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.Rect);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = Activator.CreateInstance(objectType);
            var properties = objectType.GetProperties();

            while (reader.Read() && reader.Value != null)
            {
                ConverterHelper.SetProperty(obj, reader, properties);
            }

            return obj;
        }
    }
}
