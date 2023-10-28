using System;
using System.Linq;
using Bro.Json;

namespace Bro.Toolbox.Client
{
    public class EnumTypeParser : ITypeParser
    {
        public object Convert(object objToConvert, Type typeToConvertTo)
        {
            if (!typeToConvertTo.IsEnum)
            {
                return null;
            }

            // return minimal enum value if it wasn't specified 
            if (string.IsNullOrEmpty(objToConvert as string))
            {
                var enumValues = Enum.GetValues(typeToConvertTo);
                var minIndex = 0;
                for (int j = 0; j < enumValues.Length; j++)
                {
                    var val = (int) enumValues.GetValue(j);
                    if (val < (int) enumValues.GetValue(minIndex))
                    {
                        minIndex = j;
                    }
                }
                return Enum.GetValues(typeToConvertTo).GetValue(minIndex);
            }

            try
            {
                if (Attribute.GetCustomAttribute(typeToConvertTo, typeof(JsonConverterAttribute)) == null)
                {
                    return Enum.Parse(typeToConvertTo, (string) objToConvert, true);
                }
                else
                {
                    return Enum.GetValues(typeToConvertTo)
                        .Cast<Enum>()
                        .FirstOrDefault(v => v.GetJsonProperty() == (string)objToConvert);
                }
                


            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError($"Can't convert object {objToConvert} to {typeToConvertTo}. Details:\n{e}");
                return null;
            }
        }
    }
}