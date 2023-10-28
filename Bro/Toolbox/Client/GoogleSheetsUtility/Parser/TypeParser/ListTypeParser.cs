using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class ListTypeParser : ITypeParser
    {
        public object Convert(object objToConvert, Type typeToConvertTo)
        {
            var isList = typeToConvertTo == typeof(IList<object>) || typeToConvertTo.IsGenericType;

            if (objToConvert == null || !isList)
            {
                return null;
            }
            var source = (string) objToConvert;

            if(!source.Contains("[") || !source.Contains("]"))
            {
                return null;
            }

            var list = new List<object>();
            var items = ((string)objToConvert).Split(new char[] { '[', ';', ']' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in items)
            {
                foreach (var parser in ConfigParser.TypeParsers.Keys)
                {
                    if (parser is ListTypeParser)
                    {
                        continue;
                    }

                    var result = parser.Convert(item, ConfigParser.TypeParsers[parser]);
                    if (result != null)
                    {
                        list.Add(result);
                        break;
                    }
                }
            }
            return list;
        }
    }
}
