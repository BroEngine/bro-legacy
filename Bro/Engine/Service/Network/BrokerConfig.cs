using System.Collections.Generic;

namespace Bro.Service
{
    public class BrokerConfig
    {
        public readonly List<IServiceChannel> Channels;
        public readonly string Host;
        public readonly int Port;
        public readonly string User;
        public readonly string Password;
        public readonly string Path;
        
        public readonly IServiceChannel PrivateChannel = new PrivateChannel();

        public BrokerConfig(string host, int port, string user, string password, string path = "", List<IServiceChannel> channels = null)
        {
            Channels = channels;
            Host = host;
            Port = port;
            User = user;
            Password = password;
            Path = path;

            if (Channels == null)
            {
                Channels = new List<IServiceChannel>();
            }
            
            Channels.Add( PrivateChannel );
        }
    }
}