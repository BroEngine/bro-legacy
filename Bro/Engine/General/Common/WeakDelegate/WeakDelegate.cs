using System;
using System.Reflection;

namespace Bro
{
    public class WeakDelegate<TArg>
    {
        private WeakReference _targetRef;
        private bool _isStaticMethod;
        private MethodInfo _methodInfo;
        private bool _isSet;

        public bool IsSet
        {
            get { return _isSet; }
        }

        public WeakDelegate()
        {
        }

        public WeakDelegate(Action<TArg> handler)
        {
            Set(handler);
        }

        private void Set(Action<TArg> handler)
        {
            _isStaticMethod = handler.Target == null;
            _targetRef = _isStaticMethod ? null : new WeakReference(handler.Target);
            _methodInfo = handler.Method;
            _isSet = true;
        }

        private void Clear()
        {
            _isStaticMethod = false;
            _targetRef = null;
            _methodInfo = null;
            _isSet = false;
        }

        public bool Invoke(TArg msg)
        {
            bool wasInvoked = false;
            if (!_isSet)
            {
                return wasInvoked;
            }

            if (_isStaticMethod)
            {
                _methodInfo.Invoke(null, new object[] {msg});
                wasInvoked = true;
            }
            else
            {
                if (_targetRef != null && _targetRef.IsAlive)
                {
                    _methodInfo.Invoke(_targetRef.Target, new object[] {msg});
                    wasInvoked = true;
                }
                else
                {
                    Clear();
                    wasInvoked = false;
                }
            }

            return wasInvoked;
        }
    }
}