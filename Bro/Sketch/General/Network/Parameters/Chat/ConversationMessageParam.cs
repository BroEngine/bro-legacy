using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class ConversationMessageParam : ParamsCollection
    {
        private readonly LongParam _timestamp = new LongParam();
        private readonly IntParam _userId = new IntParam();
        private readonly StringParam _userName = new StringParam();
        private readonly ByteParam _type = new ByteParam();
        private readonly StringParam _text = new StringParam();
        private readonly StringParam _meta = new StringParam();
        
        public ConversationMessageParam() : base(false)
        {
            AddParam(_timestamp);
            AddParam(_userId);
            AddParam(_userName);
            AddParam(_type);
            AddParam(_text);
            AddParam(_meta);
        }
        
        public ConversationMessage Value
        {
            get
            {
                return new ConversationMessage()
                {
                    Timestamp = _timestamp.Value,
                    UserId = _userId.Value,
                    UserName = _userName.Value,
                    Type = _type.Value,
                    Text = _text.Value,
                    Meta = _meta.Value
                };
            }
            set
            {
                _timestamp.Value = value.Timestamp;
                _userId.Value = value.UserId;
                _userName.Value = value.UserName;
                _type.Value = value.Type;
                _text.Value = value.Text;
                _meta.Value = value.Meta;
            }
        }
    }
}