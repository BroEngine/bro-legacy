using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class ConversationParam : ParamsCollection
    {
        private readonly LongParam _conversationId = new LongParam();
        private readonly StringParam _title = new StringParam();
        private readonly StringParam _meta = new StringParam();
        private readonly ArrayParam<IntParam> _users = new ArrayParam<IntParam>(short.MaxValue);
        
        public ConversationParam() : base(false)
        {
            AddParam(_conversationId);
            AddParam(_title);
            AddParam(_meta);
            AddParam(_users);
        }
        
        public Conversation Value
        {
            get
            {
                var conversation = new Conversation()
                {
                    ConversationId = _conversationId.Value,
                    Title = _title.Value,
                    Meta = _meta.Value
                };

                foreach (var userParam in _users.Params)
                {
                    conversation.Users.Add(userParam.Value);   
                }
                
                return conversation;
            }
            set
            {
                _conversationId.Value = value.ConversationId;
                _title.Value = value.Title;
                _meta.Value = value.Meta;
                
                _users.Clear();
                foreach (var userId in value.Users)
                {
                    var intParam = NetworkPool.GetParam<IntParam>();
                    intParam.Value = userId;
                    _users.Add(intParam);
                }
            }
        }
    }
}