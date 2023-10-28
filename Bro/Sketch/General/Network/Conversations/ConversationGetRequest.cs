using Bro.Network;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class ConversationGetRequest : NetworkRequest<ConversationGetRequest>
    {
        public readonly IntParam UserId = new IntParam();
        
        public ConversationGetRequest() : base(Network.Request.OperationCode.Social.ConversationGet )
        {
            AddParam(UserId);
        }
    }
}