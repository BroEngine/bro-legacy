using Bro.Network.Service;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Server.Infrastructure
{
    public class DocumentCumulativeGetResponse : ServiceResponse<DocumentCumulativeGetResponse>
    {
        public readonly ArrayParam<StringParam> Data = new ArrayParam<StringParam>(byte.MaxValue);
        
        public DocumentCumulativeGetResponse() : base(Network.Request.OperationCode.Infrastructure.DocumentCumulativeGet, new DocumentChannel())
        {
            AddParam(Data);
        }
    }
}