namespace Bro.Sketch.Network
{
    public class TimeSyncRequest : Bro.Network.NetworkRequest<TimeSyncRequest>
    {
        public TimeSyncRequest() : base(Request.OperationCode.TimeSync)
        {
            IsReliable = true;
        }
    }
}