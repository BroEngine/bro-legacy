using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class TimeSyncResponse : Bro.Network.NetworkResponse<TimeSyncResponse>
    {
        public readonly TimestampParam ServerTimestamp = new TimestampParam();

        public TimeSyncResponse() : base(Request.OperationCode.TimeSync)
        {
            AddParam(ServerTimestamp);
            IsReliable = true;
        }
    }
}