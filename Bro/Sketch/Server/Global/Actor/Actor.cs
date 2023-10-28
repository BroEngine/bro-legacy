using Bro.Server.Network;

namespace Bro.Sketch.Server
{
    public class Actor : System.IDisposable
    {
        public Profile Profile { get; set; }
        public IClientPeer Peer { get; set; }

        public readonly Inventory Inventory;
        public ConfigHolder ConfigHolder;

        public Actor()
        {
            Inventory = new Inventory(this);
        }

        public void Dispose()
        {
            Profile = null;
        }
    }
}