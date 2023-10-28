using Bro.Network;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class ConversationMessageRequest : NetworkRequest<ConversationMessageRequest>
    {
        public readonly LongParam ConversationId = new LongParam();
        public readonly ConversationMessageParam Message = new ConversationMessageParam();
        
        public ConversationMessageRequest() : base(Request.OperationCode.Social.ConversationMessage )
        {
            AddParam(ConversationId);
            AddParam(Message);
        }
    }
}
