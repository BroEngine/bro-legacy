using System;

namespace Bro.Sketch.Server
{
    [AttributeUsage(AttributeTargets.Method)]
    public class InviteEventHandlerAttribute : Attribute
    {
        // Actor, InviteAction, int, data
    } 
    
    [AttributeUsage(AttributeTargets.Method)]
    public class InviteHandlerAttribute : Attribute
    {
        // Actor, int, data
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class InviteCancelHandlerAttribute : Attribute
    {
        // Actor, int, data
    }  
    
    [AttributeUsage(AttributeTargets.Method)]
    public class InviteAcceptHandlerAttribute : Attribute
    {
        // Actor, int, data
    } 
    
    [AttributeUsage(AttributeTargets.Method)]
    public class InviteDenyHandlerAttribute : Attribute
    {
        // Actor, int, data
    }
}