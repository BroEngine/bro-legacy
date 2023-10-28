using System;

namespace Bro.Sketch.Client
{
    public class InventoryItem<T> : IInventoryItem
    {
        public delegate void ChangeValue(T previousValue, T newValue);

        public event ChangeValue OnChange;

        private bool _isInitialized;
        private T _value;

        public bool IsInitialized => _isInitialized;
        
        public T Value
        {
            set
            {
                var prevValue = _value;
                _value = (T) value;
                try
                {
                    OnChange?.Invoke(prevValue, _value);
                }
                catch (Exception e)
                {
                    Bro.Log.Error(e);
                }
                _isInitialized = true;
            }
            get
            {
                if (!_isInitialized)
                {
                    Bro.Log.Error("inventory item: trying to use not initialized inventory item " + GetType());
                }
                return _value;
            }
        }


        object IInventoryItem.Value
        {
            set => Value = (T)value;
            get => Value;
        }
    }
}