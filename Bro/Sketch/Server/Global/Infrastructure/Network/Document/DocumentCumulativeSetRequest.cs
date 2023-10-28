using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class DocumentCumulativeSetRequest : ServiceRequest<DocumentCumulativeSetRequest>
    {
        public readonly StringParam Token = new StringParam();
        public readonly StringParam Data = new StringParam();
        
        public DocumentCumulativeSetRequest() : base(Network.Request.OperationCode.Infrastructure.DocumentCumulativeSet, new DocumentChannel() )
        {
            AddParam(Token);
            AddParam(Data);
        }
    }
}