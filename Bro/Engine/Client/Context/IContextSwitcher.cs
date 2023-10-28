using System;

namespace Bro.Client
{
    public interface IContextSwitcher
    {
        void Switch(IClientContext a, IClientContext b, Action onComplete = null);
    }
}