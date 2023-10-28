namespace Bro.Network.TransmitProtocol
{
    public class EndpointParam : BaseParam
    {
        private string _value;
       
        public string Value
        {
            get
            {
                CheckInitialized();
                return _value;
            }
            set
            {
                _value = value;
                IsInitialized = true;
            }
        }

        public EndpointParam(bool isOptional = false) : base(isOptional)
        {
           
        }

        public override void Write(IWriter writer)
        {
            var endpoint = new byte[4] { 0, 0, 0, 0 };

            if (!string.IsNullOrEmpty(_value))
            {
                var data = _value.Split('.');
                if (data.Length == 4)
                {
                    for (var i = 0; i < data.Length; ++i)
                    {
                        endpoint[i] = byte.Parse(data[i]);
                    }
                }
            }
            
            writer.Write(endpoint);
        }

        public override void Read(IReader reader)
        {
            byte[] endpoint;;
            reader.Read(out endpoint, 4);

            _value = endpoint[0] + "." + endpoint[1] + "." + endpoint[2] + "." + endpoint[3];
            
            IsInitialized = true;
        }

        public override void Cleanup()
        {
            _value = string.Empty;
            base.Cleanup();
        }
    }
}