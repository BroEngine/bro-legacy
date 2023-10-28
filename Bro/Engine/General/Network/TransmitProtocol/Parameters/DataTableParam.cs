namespace Bro.Network.TransmitProtocol
{
    [UniversalParamRegistration(typeof(DataTable), UniversalParamTypeIndex.DataTable)]
    public class DataTableParam : BaseParam, IObjectParam
    {
        private DataTable _value;

        public DataTableParam() : this(false)
        {
            
        }
        public DataTableParam(bool isOptional = false) : base(isOptional)
        {
        }

        public override void Write(IWriter writer)
        {
            if (_value == null)
            {
                Log.Error($"DataTableParam value not set, debug data = {DebugData}" );
                return;
            }

            short valuesCount = (short) _value.Count;
            writer.Write(valuesCount);
            var universalParam = new UniversalParam() {SupportUnknownTypes = true};
            foreach (var pair in _value)
            {
                universalParam.Value = pair.Key;
                universalParam.Write(writer);
                universalParam.Value = pair.Value;
                universalParam.Write(writer);
            }
        }

        public override void Read(IReader reader)
        {
            reader.Read(out short valuesCount);
            var universalParam = new UniversalParam() {SupportUnknownTypes = true};
            _value = new DataTable();
            object key;
            for (int i = 0; i < valuesCount; ++i)
            {
                universalParam.Read(reader);
                key = universalParam.Value;
                universalParam.Read(reader);
                _value[key] = universalParam.Value;
            }
            
            IsInitialized = true;
        }

        object IObjectParam.Value
        {
            set => Value = (DataTable) value;
            get => Value;
        }
        
        System.Type IObjectParam.ValueType => typeof(DataTable);

        public DataTable Value
        {
            get 
            {
                if (_value == null)
                {
                    _value = new DataTable();
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
                    _value = new DataTable();
                }
                else
                {
                    _value = (DataTable) value.Clone();
                }
                IsInitialized = true;
            }
        }

        public override void Cleanup()
        {
            _value = null;
            base.Cleanup();
        }
    }
}