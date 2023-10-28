using System;

namespace Bro.Toolbox.Client
{
    public class StringTypeParser : ITypeParser
    {
        public object Convert(object objToConvert, Type typeToConvertTo)
        {
            if (objToConvert == null || typeToConvertTo != typeof(string))
            {
                return null;
            }

            try
            {
                return System.Convert.ToString(objToConvert);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"Can't convert object {objToConvert} to {typeToConvertTo}. Details:\n{e}");
                return null;
            }
        }
    }
}
