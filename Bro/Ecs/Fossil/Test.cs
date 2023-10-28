using System;

namespace Bro.Fossil
{
    public static class Test
    {
        private static readonly System.Random Random = new System.Random();
        private static readonly Delta Delta = new Delta();

        private static readonly byte[] ABuffer = new byte[Delta.BufferSize];
        private static readonly byte[] BBuffer = new byte[Delta.BufferSize];
        private static readonly byte[] DeltaBuffer = new byte[Delta.BufferSize];
        private static readonly byte[] ResultBuffer = new byte[Delta.BufferSize];
        
        public static void Test01(int size = 2048)
        {
            var a = FillRandomByteArray(ABuffer, size);
            var b = FillRandomByteArray(BBuffer, size);
            
            var deltaSize = Delta.Create(ABuffer, a, BBuffer, b, DeltaBuffer);
            var resultSize = Delta.Apply(ABuffer, a, DeltaBuffer, deltaSize, ResultBuffer);

            var ok = UnsafeCompare(BBuffer, ResultBuffer, resultSize);
            Bro.Log.Info("result = " + ok + "; delta size = " + deltaSize + "; origin size = " + a);
        }  
        
        public static void Test02(int size = 2048)
        {
            if (size == 0)
            {
                return;
            }
            
            var a = FillRandomByteArray(ABuffer, size);
            var b = a;
            Buffer.BlockCopy(ABuffer, 0, BBuffer, 0, size);
            
            BBuffer[size / 10] = 1;
            BBuffer[size / 100] = 2;
            BBuffer[size / 200] = 3;
            BBuffer[size / 1000] = 4;
            
            var deltaSize = Delta.Create(ABuffer, a, BBuffer, b, DeltaBuffer);
            var resultSize = Delta.Apply(ABuffer, a, DeltaBuffer, deltaSize, ResultBuffer);
            
            var ok = UnsafeCompare(BBuffer, ResultBuffer, resultSize);
            Bro.Log.Info("result = " + ok + "; delta size = " + deltaSize + "; origin size = " + a);
        }

        private static int FillRandomByteArray(byte[] b, int size)
        {
            // var b = new byte[size];
            Random.NextBytes(b);
            // return b;
            return size;
        }
        
        static unsafe bool UnsafeCompare(byte[] a, byte[] b, int size) 
        {
            fixed (byte* p1=a, p2=b) 
            {
                byte* x1=p1, x2=p2;
                for (var i=0; i < size/8; i++, x1+=8, x2+=8)
                    if (*((long*)x1) != *((long*)x2)) return false;
                if ((size & 4)!=0) { if (*((int*)x1)!=*((int*)x2)) return false; x1+=4; x2+=4; }
                if ((size & 2)!=0) { if (*((short*)x1)!=*((short*)x2)) return false; x1+=2; x2+=2; }
                if ((size & 1)!=0) if (*((byte*)x1) != *((byte*)x2)) return false;
                return true;
            }
        }
        
        private static string PrintByteArray(byte[] a)
        {
            var str = "";
            foreach ( var b in a)
            {
                str += " " + b;
            }  
            return str;
        }
    }
}