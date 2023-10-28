namespace Bro
{
    public static class ObjectExtensions
    {
        public static void CallMethod<T>(this object obj,string methodName)
        {
            var method = ((T)obj).GetType().GetMethod(methodName);
            if (method != null)
            {
                method.Invoke(obj, null); 
            }
            else
            {
                Bro.Log.Error($"object extensions :: no method {methodName} in {obj.GetType()}");
            }
        }
    }
}