using System;

namespace Bro.Network.TransmitProtocol
{
    public class TypeBindedParam : ParamsCollection
    {
        private readonly StringParam _classTypeKey = new StringParam();

        public TypeBindedParam()
        {
            AddParam(_classTypeKey);
        }
        
        public Type Value
        {
            get
            {
                CheckInitialized();
                return ClassTypeBinder.GetParamData(_classTypeKey.Value);
            }
            set
            {
                _classTypeKey.Value = ClassTypeBinder.GetParamData(value);
                if (_classTypeKey.Value != null)
                {
                    IsInitialized = true;
                }
                else
                {
                    Bro.Log.Error($"class type param :: key {value} is not register");
                    IsInitialized = false;
                }
            }
        }
    }
}