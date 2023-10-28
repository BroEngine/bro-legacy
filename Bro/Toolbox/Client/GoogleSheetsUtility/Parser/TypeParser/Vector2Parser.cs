using System;
using System.Collections.Generic;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class Vector2Parser : BaseVectorParser, ITypeParser
    {
        public object Convert(object objToConvert, Type typeToConvertTo)
        {
            if (objToConvert == null || typeToConvertTo != typeof(Vector2))
            {
                return null;
            }
            var coordinates = ParseFloatCoordinates(objToConvert);

            if (coordinates.Count == 2)
            {
                return new Vector2(coordinates[0], coordinates[1]);
            }
            else
            {
                return null;
            }
        }
    }
}