using Bro.Json;

namespace Bro.Network.TransmitProtocol
{
    public class JsonParam<T> : ParamsCollection, IObjectParam where T : class
    {
        private readonly StringParam _jsonData = new StringParam();
        private JsonSerializerSettings _settings = JsonSettings.ObjectsSettings;

        public T Value
        {
            get => JsonConvert.DeserializeObject<T>(_jsonData.Value, _settings);
            set => _jsonData.Value = JsonConvert.SerializeObject(value, Formatting.None, _settings);
        }

        object IObjectParam.Value { get => Value; set => Value = (T) value; }
        
        System.Type IObjectParam.ValueType => typeof(T);

        public JsonParam() : this(null, false)
        {
        }

        public JsonParam(JsonSerializerSettings settings = null, bool isOptional = false) : base(isOptional)
        {
            AddParam(_jsonData);
            if (settings != null)
            {
                _settings = settings;
            }
        }

        public void SetJsonSettings(JsonSerializerSettings settings)
        {
            _settings = settings;
        }
    }
}