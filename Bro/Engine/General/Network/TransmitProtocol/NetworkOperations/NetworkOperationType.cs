using System.ComponentModel;

namespace Bro.Network
{
    public enum NetworkOperationType : byte // max = 8 
    {
        [Description("request")] Request,
        [Description("response")] Response,
        [Description("event")] Event,
        [Description("ping")] Ping,
        [Description("encryption")] Encryption,
        [Description("handshake")] Handshake
    }
}