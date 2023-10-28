using System;
using Bro.Server.Network;

namespace Bro.Sketch.Server
{
    public interface IProfileProvider
    {
        bool Load(UserIdentity identity, IClientPeer peer, bool resetProfile, Action<Profile, byte> callback);
        void Save(Profile profile, IClientPeer peer, Action<bool> callback);
        bool IsQueueBusy();
    }
}