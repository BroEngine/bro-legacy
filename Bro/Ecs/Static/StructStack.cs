namespace Bro.Ecs
{
    public class StructStack<T> where T : struct /* ! MAX SIZE 64 ! */
    {
        private const byte Size = 64;
        
        private ulong _mask;
        private readonly T[] _data = new T[Size];

        private byte PopFreeIndex()
        {
            for (byte i = 0; i < Size; i++)
            {
                var bite = (ulong) (1L << i);
                if ((_mask & bite) != bite)
                {
                    _mask |= bite; // bite
                    return i; // index
                }
            }

            Bro.Log.Error("over sized");
            _mask |= 1; // bite
            return 0; // index
        }   
        
        private void TakeIndex(byte index)
        {
            Bro.Log.Assert(index < Size);
            var bite = (ulong) (1L << index);
            _mask |= bite;
        } 
        
        private void FreeIndex(byte index)
        {
            Bro.Log.Assert(index < Size);
            var bite = (ulong) (1L << index);
            _mask &= (_mask ^ bite);
        }

        public ref T Push()
        {
            return ref _data[PopFreeIndex()];
        }  
        
        public bool Pop(out T item)
        {
            for (byte i = 0; i < Size; i++)
            {
                var bite = (ulong) (1L << i);
                if ((_mask & bite) == bite)
                {
                    _mask &= (_mask ^ bite); // free
                    item = _data[i];
                    return true; 
                }
            }

            item = _data[0];
            return false;
        }

        public ref T Set(byte index)
        {
            TakeIndex(index);
            return ref _data[index];
        } 
        
        public ref T Get(byte index)
        {
            return ref _data[index];
        }

        public void Remove(byte index)
        {
            FreeIndex(index);
        }

        public void Clear()
        {
            _mask = 0;
        }

        /*  Example:
         * 
         *  var stack = new StructStack<Test>();
         *  for (var i = 0; i < 30; ++i)
         *  {
         *      stack.Push().Index = i;
         *  }
         *
         *  Test item;
         *  while (stack.Pop(out item))
         *  {
         *      Bro.Log.Info("index = " + item.Index);
         *  }
         */
        
        public void Debug()
        {
            Debug(_mask);
        } 
        
        private void Debug(ulong mask)
        {
            var maskStr = string.Empty;
            for (byte i = 0; i < Size; i++)
            {
                var bite = (ulong) (1L << i);
                var isDominant = (mask & bite) == bite;
                maskStr += (isDominant ? "1 " : "0 ");
            }
            
            Bro.Log.Info("mask = " + maskStr);
        }
    }
}