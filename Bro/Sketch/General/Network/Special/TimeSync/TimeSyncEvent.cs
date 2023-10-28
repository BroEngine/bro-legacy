using Bro.Network.TransmitProtocol;

namespace Bro.Sketch.Network
{
    public class TimeSyncEvent : Bro.Network.NetworkEvent<TimeSyncEvent>
    {
        public readonly TimestampParam ServerTimestamp = new TimestampParam();

        public TimeSyncEvent() : base(Event.OperationCode.TimeSync)
        {
            AddParam(ServerTimestamp);
            IsReliable = false;
            IsOrdered = false;
        }
    }
}