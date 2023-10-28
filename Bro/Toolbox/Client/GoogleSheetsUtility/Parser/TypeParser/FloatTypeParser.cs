using System;
using System.Globalization;

namespace Bro.Toolbox.Client
{
    public class FloatTypeParser : ITypeParser
    {
        public object Convert(object objToConvert, Type typeToConvertTo)
        {
            if (objToConvert == null || typeToConvertTo != typeof(float))
            {
                return null;
            }

            try
            {
                return System.Convert.ToSingle(objToConvert,new CultureInfo("en")
                {
                    NumberFormat =
                    {
                        NumberDecimalSeparator = ","
                    }
                });
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"Can't convert object {objToConvert} to {typeToConvertTo}. Details:\n{e}");
                return null;
            }
        }
    }
}
