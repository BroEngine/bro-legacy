using System;

namespace Bro.Client
{
    public class ContextSwitcher : IContextSwitcher
    {
        private readonly ClientApplication _application;

        public ContextSwitcher(ClientApplication application)
        {
            _application = application;
        }

        public void Switch(IClientContext a, IClientContext b, Action onComplete = null)
        {
            if (a != null)
            {
                a?.Unload(() =>
                {
                    b.Load(_application);
                    onComplete?.Invoke();
                });
            }
            else
            {
                b.Load(_application);
                onComplete?.Invoke();
            }
        }
    }
}