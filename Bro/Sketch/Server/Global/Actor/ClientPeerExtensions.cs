using System.Runtime.CompilerServices;
using Bro.Server.Network;

namespace Bro.Sketch.Server
{
    public static class ClientPeerExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Actor GetActor(this IClientPeer peer)
        {
            return peer.PeerData as Actor;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetActor(this IClientPeer peer, Actor actor)
        {
            peer.PeerData = actor;
        }
    }
}