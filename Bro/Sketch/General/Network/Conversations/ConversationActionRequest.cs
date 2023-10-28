using Bro.Network;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class ConversationActionRequest : NetworkRequest<ConversationActionRequest>
    {
        public readonly LongParam ConversationId = new LongParam();
        public readonly IntParam UserId = new IntParam();
        public readonly ByteParam Action = new ByteParam();
        public readonly StringParam Title = new StringParam();
        public readonly StringParam Meta = new StringParam();
        
        public ConversationActionRequest() : base(Network.Request.OperationCode.Social.ConversationAction )
        {
            AddParam(ConversationId);
            AddParam(UserId);
            AddParam(Action);
            AddParam(Title);
            AddParam(Meta);
        }
    }
}