using System;

namespace Bro.Client
{
    public interface IEventObserver : IDisposable
    {
        int EventId { get; }
        bool OnEvent(IEvent e);
    }
}