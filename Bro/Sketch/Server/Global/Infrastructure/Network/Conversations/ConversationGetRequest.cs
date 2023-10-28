using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class ConversationGetRequest : ServiceRequest<ConversationGetRequest>
    {
        public readonly IntParam UserId = new IntParam();
        
        public ConversationGetRequest() : base(Network.Request.OperationCode.Social.ConversationGet, new ConversationChannel() )
        {
            AddParam(UserId);
        }
    }
}

