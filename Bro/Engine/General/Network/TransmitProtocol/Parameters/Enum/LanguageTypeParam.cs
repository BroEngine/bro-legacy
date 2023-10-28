using System;

namespace Bro.Network.TransmitProtocol
{
    [UniversalParamRegistration(typeof(LanguageType), UniversalParamTypeIndex.LanguageType)]
    public class LanguageTypeParam : GenericByteParam<LanguageType>, IObjectParam
    {
        public LanguageTypeParam(bool isOptional = false) : base(LanguageType.Unknown, isOptional)
        {
        }

        public LanguageTypeParam() : this(false)
        {
        }
        
        protected override LanguageType Convert(byte value)
        {
            return (LanguageType) value;
        }

        protected override byte Convert(LanguageType value)
        {
            return (byte) value;
        }

        object IObjectParam.Value
        {
            get => Value;
            set => Value = (LanguageType) value;
        }

        Type IObjectParam.ValueType => typeof(LanguageType);
    }
}