namespace Bro
{
    public class ObjectPool<TObject>
    {
        public bool IsEnabled = true;

        private readonly int _maxSize;
        public int MaxSize => _maxSize;
        public int ObjectsAmount { get; protected set; }
        protected readonly TObject[] Objects;
        private System.Func<TObject> _constructor;

        protected readonly object Locker = new object();

        public System.Func<TObject> Constructor
        {
            set => _constructor = value;
        }
        
        public event System.Action<TObject> Disabler;

        public event System.Action<TObject> Enabler;

        public event System.Action<TObject> Destoyer;

        public ObjectPool(int maxSize)
        {
            _maxSize = maxSize;
            Objects = new TObject[_maxSize];
        }

        public TObject Acquire(bool log = false)
        {
            lock (Locker)
            {
                TObject result;

                if (IsEnabled && ObjectsAmount > 0)
                {
                    --ObjectsAmount;
                    result = Objects[ObjectsAmount];
                    Objects[ObjectsAmount] = default(TObject);
                    Enabler?.Invoke(result);
                }
                else
                {
                    result = _constructor.Invoke();
                }

                return result;
            }
        }

        public bool Release(TObject obj)
        {
            lock (Locker)
            {
                Disabler?.Invoke(obj);
                if (!IsEnabled || ObjectsAmount >= _maxSize)
                {
                    Destoyer?.Invoke(obj);
                    return false;
                }

                Objects[ObjectsAmount] = obj;
                ++ObjectsAmount;
                return true;
            }
        }

        public void Reset()
        {
            lock (Locker)
            {
                while (ObjectsAmount > 0)
                {
                    --ObjectsAmount;
                    var o = Objects[ObjectsAmount];
                    Objects[ObjectsAmount] = default(TObject);
                    Disabler?.Invoke(o);
                    Destoyer?.Invoke(o);
                }
            }
        }
    }
    
    public class ObjectPool<TObject, TParam> : ObjectPool<TObject>
    {
        public delegate TObject CreateDelegateWithParam(TParam param);

        private CreateDelegateWithParam _constructorWithParam;

        public CreateDelegateWithParam ConstructorWithParam
        {
            set { _constructorWithParam = value; }
        }

        public ObjectPool(int maxSize) : base(maxSize)
        {
        }

        public TObject Acquire(TParam param)
        {
            TObject result;
            lock (Locker)
            {
                if (ObjectsAmount > 0)
                {
                    --ObjectsAmount;
                    result = Objects[ObjectsAmount];
                }
                else
                {
                    result = _constructorWithParam.Invoke(param);
                }
            }

            return result;
        }
    }
}