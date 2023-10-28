using System;

namespace Bro
{
    public class DisposableHandler : IDisposable
    {
        private bool _isDisposed;
        private Action _onDispose;
        
        public DisposableHandler(Action onDispose)
        {
            _onDispose = onDispose;
        }

        ~DisposableHandler()
        {
            ProcessDispose();
        }
        
        private void ProcessDispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                _onDispose();
            }
        }

        void IDisposable.Dispose()
        {
            ProcessDispose();
            GC.SuppressFinalize(this);
        }
    }
}