using System.ComponentModel;

namespace Bro.Network
{
    public enum ConnectionProtocol
    {
        [Description("undefined")] Undefined,
        [Description("udp")] Udp,
        [Description("tcp")] Tcp,
        [Description("offline")] Offline
    }
}