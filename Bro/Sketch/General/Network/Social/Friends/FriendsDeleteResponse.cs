using Bro.Network;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class FriendsDeleteResponse : NetworkResponse<FriendsDeleteResponse>
    {
        public ByteParam Result = new ByteParam();
        
        public FriendsDeleteResponse() : base(Request.OperationCode.Social.FriendsDelete)
        {
            AddParam(Result);
        }
    }
}