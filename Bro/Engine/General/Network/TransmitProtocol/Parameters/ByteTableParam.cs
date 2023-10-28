using System.Collections.Generic;

namespace Bro.Network.TransmitProtocol
{
    public class ByteTableParam : BaseParam, IObjectParam
    {
        private Dictionary<byte, object> _value;

        public ByteTableParam(bool isOptional = false) : base(isOptional)
        {
        }

        public override void Write(IWriter writer)
        {
            if (_value == null)
            {
                Bro.Log.Error("value is not set, debug data = " + DebugData);
                return;
            }

            byte valuesCount = (byte) _value.Count;
            writer.Write(valuesCount);
            var universalParam = new UniversalParam() {SupportUnknownTypes = true};
            foreach (var pair in _value)
            {
                writer.Write(pair.Key);
                universalParam.Value = pair.Value;
                universalParam.Write(writer);
            }
        }

        public override void Read(IReader reader)
        {
            reader.Read(out byte valuesCount);
            var universalParam = new UniversalParam() {SupportUnknownTypes = true};
            _value = new Dictionary<byte, object>(valuesCount);
            byte key;
            for (int i = 0; i < valuesCount; ++i)
            {
                reader.Read(out key);
                universalParam.Read(reader);
                _value[key] = universalParam.Value;
            }

            IsInitialized = true;
        }

        object IObjectParam.Value { set { Value = (Dictionary<byte, object>) value; } get { return Value; } }

        System.Type IObjectParam.ValueType => typeof(Dictionary<byte, object>);
        
        public Dictionary<byte, object> Value
        {
            get
            {
                if (_value == null)
                {
                    _value = new Dictionary<byte, object>();
                    IsInitialized = true;
                }
                else
                {
                    CheckInitialized();
                }
                return _value;
            }
            set
            {
                if (value == null)
                {
                    _value = new Dictionary<byte, object>();
                }
                else
                {
                    _value = Copy(value);
                }
                IsInitialized = true;
            }
        }

        private static Dictionary<byte, object> Copy(Dictionary<byte, object> source)
        {
            var result = new Dictionary<byte, object>(source.Count);
            foreach (var item in source)
            {
                result[item.Key] = item.Value;
            }
            return result;
        }

        public override void Cleanup()
        {
            _value = null;
            base.Cleanup();
        }
    }
}