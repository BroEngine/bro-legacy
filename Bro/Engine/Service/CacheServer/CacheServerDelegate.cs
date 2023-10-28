namespace Bro.Service
{
    public static class CacheServerDelegate
    {
        public delegate bool ValueJsonCheck(string json);
        public delegate string ValueJsonCreate(string json);
        
        
        public delegate bool ValueObjectCheck<T>(T o);
        public delegate T ValueObjectCreate<T>(T o);
    }
}