using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class DocumentTemporaryGetResponse : ServiceResponse<DocumentTemporaryGetResponse>
    {
        public readonly StringParam Data = new StringParam();
        
        public DocumentTemporaryGetResponse() : base(Network.Request.OperationCode.Infrastructure.DocumentTemporaryGet, new DocumentChannel())
        {
            AddParam(Data);
        }
    }
}