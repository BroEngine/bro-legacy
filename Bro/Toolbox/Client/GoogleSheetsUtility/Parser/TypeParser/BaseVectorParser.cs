using System;
using System.Collections.Generic;

namespace Bro.Toolbox.Client
{
    public class BaseVectorParser
    {
        protected List<float> ParseFloatCoordinates(object objToConvert)
        {
            var items = ((string) objToConvert).Split(new char[] {'{', ' ', '}'}, StringSplitOptions.RemoveEmptyEntries);
            var coordinates = new List<float>();
            foreach (var item in items)
            {
                var floatParser = new FloatTypeParser();
                var result = floatParser.Convert(item, typeof(float));
                if (result != null)
                {
                    coordinates.Add((float) result);
                }
            }

            return coordinates;
        }
    }
}