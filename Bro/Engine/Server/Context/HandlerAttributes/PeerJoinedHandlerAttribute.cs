using System;

namespace Bro.Server.Context
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PeerJoinedHandlerAttribute : Attribute
    {
    }
}