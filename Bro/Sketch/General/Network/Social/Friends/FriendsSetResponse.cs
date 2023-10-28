using Bro.Network;
using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class FriendsSetResponse : NetworkResponse<FriendsSetResponse>
    {
        public ByteParam Result = new ByteParam();
        
        public FriendsSetResponse() : base(Request.OperationCode.Social.FriendsSet)
        {
            AddParam(Result);
        }
    }
}