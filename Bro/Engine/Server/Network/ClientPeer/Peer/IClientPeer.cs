using Bro.Network;
using Bro.Server.Context;

namespace Bro.Server.Network
{
    public interface IClientPeer
    {
        int PeerId { get; }
        System.IDisposable PeerData { get; set; }

        long InactiveTimeMs { get; }

        bool Destroying { get; }

        IServerContext Context { get; set; }
        bool OnStartSwitchingContext();
        void OnFinishSwitchingContext();

        void Send(INetworkOperation r);

        void Disconnect(byte code);

        void OnStartHandleRequest();
        void OnEndHandleRequest();

        void SetEncryption(string key);
    }
}