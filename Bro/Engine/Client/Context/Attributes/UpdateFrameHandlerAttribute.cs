using System;

namespace Bro.Client
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UpdateFrameHandlerAttribute : Attribute
    {
        public UpdateFrameHandlerAttribute()
        {
           
        }
    }
}