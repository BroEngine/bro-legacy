using Bro.Json;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Bro.Sketch
{
    public class Vector3JsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var vector = (UnityEngine.Vector3)value;
            writer.WriteStartObject();
            writer.WritePropertyName("x");
            writer.WriteValue(vector.x);
            writer.WritePropertyName("y");
            writer.WriteValue(vector.y);
            writer.WritePropertyName("z");
            writer.WriteValue(vector.z);
            writer.WriteEndObject();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(UnityEngine.Vector3);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = Activator.CreateInstance(objectType);
            var fields = objectType.GetFields();

            while (reader.Read() && reader.Value != null )
            {
                ConverterHelper.SetField(obj, reader, fields);
            }

            return obj;
        }
    }
}
