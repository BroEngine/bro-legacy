using Bro.Network.Service;
using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class ConversationMessageRequest : ServiceRequest<ConversationMessageRequest>
    {
        public readonly LongParam ConversationId = new LongParam();
        public readonly ConversationMessageParam Message = new ConversationMessageParam();
        
        public ConversationMessageRequest() : base(Network.Request.OperationCode.Social.ConversationMessage, new ConversationChannel() )
        {
            AddParam(ConversationId);
            AddParam(Message);
        }
    }
}