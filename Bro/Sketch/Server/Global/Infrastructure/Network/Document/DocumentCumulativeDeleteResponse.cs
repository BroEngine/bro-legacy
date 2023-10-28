using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class DocumentCumulativeDeleteResponse : ServiceResponse<DocumentCumulativeDeleteResponse>
    {
        public readonly BoolParam Successfully = new BoolParam();
        
        public DocumentCumulativeDeleteResponse() : base(Network.Request.OperationCode.Infrastructure.DocumentCumulativeDelete, new DocumentChannel())
        {
            AddParam(Successfully);
        }
    }
}