using System;

namespace Bro.Engine.Client
{
    public class ReportingProperty<T> 
    {
        public Action<T,T> OnChange;

        private T _value;

        public T Value
        {
            get => _value;
            set
            {
                var oldValue = _value;
                _value = value;
                OnChange?.Invoke(oldValue, value);
            }
        }
    }
}