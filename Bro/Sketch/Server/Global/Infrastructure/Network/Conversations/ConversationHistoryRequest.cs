using Bro.Network.Service;
using Bro.Network.TransmitProtocol;
using Bro.Sketch.Network;

namespace Bro.Sketch.Server.Infrastructure
{
    public class ConversationHistoryRequest : ServiceRequest<ConversationHistoryRequest>
    {
        public readonly LongParam ConversationId = new LongParam();
        public readonly IntParam UserId = new IntParam();
        public readonly IntParam Size = new IntParam();

        public ConversationHistoryRequest() : base(Request.OperationCode.Social.ConversationHistory, new ConversationChannel() )
        {
            AddParam(ConversationId);
            AddParam(UserId);
            AddParam(Size);
        }
    }
}


