using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class GroupParam : ParamsCollection
    {
        private readonly LongParam _groupId = new LongParam();
        private readonly ArrayParam<IntParam> _users = new ArrayParam<IntParam>(byte.MaxValue);
        private readonly StringParam _data = new StringParam();
        private readonly LongParam _timestamp = new LongParam();

        public GroupParam() : base()
        {
            AddParam(_groupId);
            AddParam(_timestamp);
            AddParam(_users);
            AddParam(_data);
        }
        
        public Group Value
        {
            get
            {
                var group = new Group()
                {
                    GroupId = _groupId.Value,
                    Data = _data.Value,
                    Timestamp = _timestamp.Value
                };

                foreach (var userParam in _users.Params)
                {
                    group.Users.Add(userParam.Value);
                }
                
                return group;
            }
            set
            {
                _groupId.Value = value.GroupId;
                _data.Value = value.Data;
                _timestamp.Value = value.Timestamp;

                _users.Params.Clear();
                foreach (var user in value.Users)
                {
                    var intParam = NetworkPool.GetParam<IntParam>();
                    intParam.Value = user;
                    _users.Add(intParam);
                }
            }
        }  
    }
}