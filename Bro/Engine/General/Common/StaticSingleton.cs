namespace Bro
{
    public class StaticSingleton<T> where T : new()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    Initialize();
                }

                return _instance;
            }
        }
        
        public static void Initialize()
        {
            _instance = new T();
        }
        public static void Initialize(T instance)
        {
            _instance = instance;
        }
    }
    
    // ReSharper disable InconsistentNaming
    public class StaticSingleton<I,T> where T : new() where I : class
    {
        protected static T _instance;
        private static I _interface;
        
        public static I Instance
        {
            get
            {
                if (_interface == null)
                {
                    Initialize();
                }

                return _interface;
            }
        }

        private static void Initialize()
        {
            _instance = new T();
            _interface = _instance as I;
        }

        public static void ResetInstance()
        {
            _instance = default(T);
            _interface = null;
        }
    }
}