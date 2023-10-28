namespace Bro.Server.Network
{
    public interface IPeerContainer
    {
        void ForEachPeer(System.Action<IClientPeer> action);
        int PeersAmount { get; }
    }
}