using System;
using System.Collections.Generic;
using System.Linq;

namespace Bro
{
    public class Random
    {
        private readonly LaggedFibonacci _rnd;

        public static readonly Random Instance = new Random();

        public int Seed => _rnd.Seed;

        public Random()
        {
            _rnd = new LaggedFibonacci( (int)( TimeInfo.GlobalTimestamp % ( 1024 * 1024 ) ) );
        }

        public Random(int seed)
        {
            _rnd = new LaggedFibonacci( seed );
        }

        /// <summary>
        /// example: min = 1, max = 3 will give results 1 or 2
        /// </summary>
        /// <param name="min">included in range</param>
        /// <param name="max">Not incuded in range</param>
        /// <returns></returns>
        public int Range(int min, int max)
        {
            if (min >= max)
            {
                return min;
            }

            var range = (max - min);
            return Math.Abs( _rnd.NextInteger() % range ) + min;
        }

        public float Range(float min, float max)
        {
            if (min >= max)
            {
                return min;
            }

            var range = (max - min);
            return Math.Abs( (float) _rnd.NextFloat() * range ) + min;
        }

        public int Range(Range<int> r)
        {
            return Range(r.Min, r.Max);
        }
        
        public float Range(Range<float> r)
        {
            return Range(r.Min, r.Max);
        }

        
        public int[] Field(int size, int sum)
        {
            int LOWERBOUND = 1;
            int UPPERBOUND = (int) System.Math.Ceiling((float) sum / (float) size);


            int[] result = new int[size];
            int currentsum = 0;
            int low, high, calc;

            if ((UPPERBOUND * size) < sum || (LOWERBOUND * size) > sum || UPPERBOUND < LOWERBOUND)
            {
                return new int[0];
            }

            for (int index = 0; index < size; index++)
            {
                calc = (sum - currentsum) - (UPPERBOUND * (size - 1 - index));
                low = calc < LOWERBOUND ? LOWERBOUND : calc;
                calc = (sum - currentsum) - (LOWERBOUND * (size - 1 - index));
                high = calc > UPPERBOUND ? UPPERBOUND : calc;

                result[index] = Instance.Range(low, high + 1);

                currentsum += result[index];
            }

            int shuffleCount = Instance.Range(size * 5, size * 10);
            while (shuffleCount-- > 0)
                Swap(ref result[Instance.Range(0, size)], ref result[Instance.Range(0, size)]);

            return result;
        }

        private void Swap(ref int item1, ref int item2)
        {
            int temp = item1;
            item1 = item2;
            item2 = temp;
        }

        public string String(int lenght = 16)
        {
            const string _glyphRange = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm0123456789";
            var uid = string.Empty;
            for (var i = 0; i < lenght; ++i)
            {
                uid += _glyphRange[Range(0, _glyphRange.Length)];
            }

            return uid;
        }
        
       
    }

    public static class RandomExtention
    {
        private static readonly System.Random Random = new System.Random();  
        
        public static void Shuffle<T>(this IList<T> list)  
        {  
            var n = list.Count;  
            while (n > 1) {  
                n--;  
                var k = Random.Next(n + 1);  
                var value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }  
        }

        public static T RandomItem<T>(this IList<T> list)
        {
            var index = Random.Next(list.Count);
            return list[index];
        }
        
        public static T RandomItem<T>(this IEnumerable<T> list)
        {
            var index = Random.Next(list.Count());
            return list.ElementAt(index);
        }
    }
}