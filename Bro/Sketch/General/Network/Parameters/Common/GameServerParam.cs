using Bro.Network.TransmitProtocol;

namespace Bro.Sketch
{
    public class GameServerParam : ParamsCollection
    {
        private readonly IntParam _serverId = new IntParam();
        private readonly StringParam _type = new StringParam();
        private readonly StringParam _ip = new StringParam();
        private readonly IntParam _port = new IntParam();
        private readonly IntParam _version = new IntParam();
        
        public GameServerParam() : base(false)
        {
            AddParam(_serverId);
            AddParam(_type);
            AddParam(_ip);
            AddParam(_port);
            AddParam(_version);
        }
        
        public GameServer Value
        {
            get
            {
                if (!IsInitialized)
                {
                    Bro.Log.Error("Can not create room passport from params, case they are not initialized");
                }

                var server = new GameServer
                {
                    ServerId = _serverId.Value,
                    Type = _type.Value,
                    Ip = _ip.Value,
                    Port = _port.Value,
                    Version = _version.Value
                };

                return server;
            }
            set
            {
                _serverId.Value = value.ServerId;
                _type.Value = value.Type;
                _ip.Value = value.Ip;
                _port.Value = value.Port;
                _version.Value = value.Version;

                IsInitialized = true;
            }
        }
    }
}