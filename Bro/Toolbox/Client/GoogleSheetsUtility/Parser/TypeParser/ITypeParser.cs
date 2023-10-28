using System;

namespace Bro.Toolbox.Client
{
    public interface ITypeParser
    {
        public object Convert(object objToConvert, Type typeToConvertTo);
    }
}
