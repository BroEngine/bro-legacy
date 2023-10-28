using Bro.Json;
using System;
using System.Linq;
using System.Reflection;

namespace Bro.Sketch
{
    public static class ConverterHelper
    {
        public static void SetField(object obj, JsonReader reader, FieldInfo[] fields)
        {
            var field = fields.FirstOrDefault(f => f.Name.Equals(reader.Value)); // reader should have value allready
            reader.Read(); // read field value
            if (field != null)
            {
                field.SetValue(obj, Convert.ToSingle(reader.Value));
            }
        }

        public static void SetProperty(object obj, JsonReader reader, PropertyInfo[] properties)
        {
            var property = properties.FirstOrDefault(f => f.Name.Equals(reader.Value)); // reader should have value allready
            reader.Read(); // read field value
            if (property != null)
            {
                property.SetValue(obj, Convert.ToSingle(reader.Value));
            }
        }
    }
}
