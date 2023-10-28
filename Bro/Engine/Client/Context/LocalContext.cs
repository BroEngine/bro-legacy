using System;

namespace Bro.Client
{
    public class LocalContext
    {
        private readonly ClientApplication _application;
        private bool _isSwitching;
        private IContextSwitcher _contextSwitcher;
        
        public IClientContext CurrentContext { get; private set; }

        public LocalContext(ClientApplication application)
        {
            _application = application;
            _contextSwitcher = new ContextSwitcher(_application); 
        }

        public void SetContextSwitcher(IContextSwitcher contextSwitcher)
        {
            _contextSwitcher = contextSwitcher;
        }

        public void SwitchTo(IClientContext newContext, Action callback = null)
        {
            if (_isSwitching)
            {
                Bro.Log.Error("active context :: cannot switch because already switching");
                return;
            }
            
            var prevContext = CurrentContext;
            
            _isSwitching = true;
            
            CurrentContext = null;
            _contextSwitcher.Switch(prevContext, newContext, callback);
            CurrentContext = newContext;
            
            _isSwitching = false;
        }
    }
}