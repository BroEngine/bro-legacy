using System.Collections.Generic;
using System.Net.Http;

namespace Bro.Sketch.Server.Infrastructure
{
    public static class Broadcaster
    {
        public static void Publish(ServerConfig serverConfig, string message)
        {
            var values = new Dictionary<string, string>
            {
                { "message", message }
            };
            var content = new FormUrlEncodedContent(values);
            var client = new HttpClient();
       
            
        }
    }
}