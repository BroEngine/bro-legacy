using System;

namespace Bro.Toolbox.Client
{
    public class BaseTypeParser : ITypeParser
    {
        public object Convert(object objToConvert, Type typeToConvertTo)
        {
            if (objToConvert == null)
            {
                return null;
            }

            try
            {
                return System.Convert.ChangeType(objToConvert, typeToConvertTo);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"Can't convert object {objToConvert} to {typeToConvertTo}. Details:\n{e}");
                return null;
            }
        }
    }
}
