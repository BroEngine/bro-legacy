using System.Runtime.CompilerServices;

namespace Bro.Ecs
{
    public static class FastArrayComparer
    {
        public static bool Compare<T>(T[] aArray, T[] bArray, int size) where T : struct /* ! MAX SIZE 64 ! */
        {
            ulong mask = 0L;
            for (var i = 0; i < size; ++i) {
 
                int j;
                for (j = 0; j < size; j++)
                {
                    var bite = (ulong) (1L << j);
                    if (aArray[i].Equals(bArray[j]) && (mask & bite) != bite)
                    {
                        mask |= bite;
                        break;           
                    }  
                }
                if (j == size)
                {
                    return false;
                }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Compare(object[] aArray,object[] bArray, int size) /* ! MAX SIZE 64 ! */
        {
            ulong mask = 0L;
            for (var i = 0; i < size; ++i) {
 
                int j;
                for (j = 0; j < size; j++)
                {
                    var bite = (ulong) (1L << j);
                    if (aArray[i].Equals(bArray[j]) && (mask & bite) != bite)
                    {
                        mask |= bite;
                        break;           
                    }  
                }
                if (j == size)
                {
                    return false;
                }
            }
            return true;
        }

        public static void Test()
        {
            var size = 64;
            var a = new object[size];
            var b = new object[size];
            for (var i = 0; i < size; i++)
            {
                a[i] = i;
                b[i] = size - i - 1;
            }
            
            Bro.Log.Error("test result = " + Compare(a, b, size));
        }
    }
}