using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class UserStatusParam : ParamsCollection
    {
        private readonly IntParam _userId = new IntParam();
        private readonly IntParam _sessionId = new IntParam(isOptional:true);
        private readonly ByteParam _state = new ByteParam(isOptional:true);        
        private readonly LongParam _timestamp = new LongParam(isOptional:true);
        private readonly StringParam _channel = new StringParam(isOptional:true);
        private readonly StringParam _name = new StringParam(isOptional:true);
        private readonly StringParam _data = new StringParam(isOptional:true);
  
        public UserStatusParam(bool isOptional = false) : base(isOptional)
        {
            AddParam(_userId);
            AddParam(_sessionId);
            AddParam(_state);
            AddParam(_timestamp);
            AddParam(_channel);
            AddParam(_name);
            AddParam(_data);
        }
        
        public UserStatus Value
        {
            get
            {
                if (!IsInitialized)
                {
                    Log.Error("Can not create player status from params, case they are not initialized");
                }

                return new UserStatus()
                {
                    UserId = _userId.Value,
                    SessionId = _sessionId.Value,
                    State = _state.Value,
                    Timestamp = _timestamp.ValueOrDefault,
                    Channel = _channel.Value,
                    Name = _name.ValueOrDefault,
                    Data = _data.Value,
                };
            }
            set
            {
                _userId.Value = value.UserId;
                _sessionId.Value = value.SessionId;
                _state.Value = value.State;
                _timestamp.Value = value.Timestamp;
                _channel.Value = value.Channel;
                _name.Value = value.Name;
                _data.Value = value.Data;
            }
        }
    }
}