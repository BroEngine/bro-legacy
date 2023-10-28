// ----------------------------------------------------------------------------
// The MIT License
// Lightweight ECS framework https://github.com/Leopotam/ecslite
// Copyright (c) 2021-2022 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

namespace Leopotam.EcsLite
{
    public static class EcsTools
    {
        public static void InsertionSort(int[] items, int len) // no allocation
        {
            for (var i = 0; i < len; ++i)
            {
                var current = items[i];
                for (var j = i - 1; j >= 0 && ! ( current > items[j] ); --j)
                {
                    items[j+1] = items[j];
                    items[j] = current;
                }
            }
        }
    }
}