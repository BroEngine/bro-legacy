using System;

namespace Bro.Server.Context
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PeerDisconnectedHandlerAttribute : Attribute
    {
    }
}