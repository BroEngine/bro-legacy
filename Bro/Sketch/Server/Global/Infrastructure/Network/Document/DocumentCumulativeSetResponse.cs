using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class DocumentCumulativeSetResponse : ServiceResponse<DocumentCumulativeSetResponse>
    {
        public readonly BoolParam Successfully = new BoolParam();
        
        public DocumentCumulativeSetResponse() : base(Network.Request.OperationCode.Infrastructure.DocumentCumulativeSet, new DocumentChannel())
        {
            AddParam(Successfully);
        }
    }
}