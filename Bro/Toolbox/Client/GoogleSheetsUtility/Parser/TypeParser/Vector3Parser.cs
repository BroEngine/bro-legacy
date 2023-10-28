using System;
using UnityEngine;

namespace Bro.Toolbox.Client
{
    public class Vector3Parser : BaseVectorParser, ITypeParser
    {
        public object Convert(object objToConvert, Type typeToConvertTo)
        {
            if (objToConvert == null || typeToConvertTo != typeof(Vector3))
            {
                return null;
            }
            
            var coordinates = ParseFloatCoordinates(objToConvert);

            if (coordinates.Count == 3)
            {
                return new Vector3(coordinates[0], coordinates[1], coordinates[2]);
            }
            else
            {
                return null;
            }
        }
    }
}