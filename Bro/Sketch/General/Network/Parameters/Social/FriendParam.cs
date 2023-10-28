using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class FriendParam : ParamsCollection
    {
        private readonly IntParam _userId = new IntParam();
        private readonly IntParam _authorUserId = new IntParam();
        private readonly ByteParam _state = new ByteParam();
        private readonly LongParam _timestamp = new LongParam();
        private readonly UserStatusParam _status = new UserStatusParam();
        
        public FriendParam() : base(false)
        {
            AddParam(_userId);
            AddParam(_authorUserId);
            AddParam(_state);
            AddParam(_timestamp);
            AddParam(_status);
        }

        public Friend Value
        {
            get
            {
                if (!IsInitialized)
                {
                    Bro.Log.Error("Can not create friend from params, case they are not initialized");
                }
                
                return new Friend()
                {
                    UserId = _userId.Value,
                    AuthorUserId = _authorUserId.Value,
                    State = (FriendState) _state.Value,
                    Timestamp = _timestamp.Value,
                    Status = _status.Value
                };
            }
            set
            {
                _userId.Value = value.UserId;
                _authorUserId.Value = value.AuthorUserId;
                _state.Value = (byte) value.State;
                _timestamp.Value = value.Timestamp;
                _status.Value = value.Status;
            }
        }  
    }
}