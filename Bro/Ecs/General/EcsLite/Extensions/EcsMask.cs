// ----------------------------------------------------------------------------
// The MIT License
// Lightweight ECS framework https://github.com/Leopotam/ecslite
// Copyright (c) 2021-2022 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;

namespace Leopotam.EcsLite
{
    public sealed class EcsMask 
    {
        readonly EcsWorld _world;
        internal int[] Include;
        internal int[] Exclude;
        internal int IncludeCount;
        internal int ExcludeCount;
        internal int Hash;
        
        internal EcsMask (EcsWorld world) 
        {
            _world = world;
            Include = new int[8];
            Exclude = new int[2];
            Reset ();
        }

        internal void Set(EcsMask sourceEcsMask)
        {
            if (sourceEcsMask.IncludeCount > IncludeCount) 
            {
                Array.Resize (ref Include, sourceEcsMask.IncludeCount);
            }

            for (int i = 0, iMax = sourceEcsMask.IncludeCount; i < iMax; i++) {
                Include[i] = sourceEcsMask.Include[i];
            }
            IncludeCount = sourceEcsMask.IncludeCount;
            
            for (int i = 0, iMax = sourceEcsMask.ExcludeCount; i < iMax; i++) {
                Exclude[i] = sourceEcsMask.Exclude[i];
            }
            ExcludeCount = sourceEcsMask.ExcludeCount;
            Hash = sourceEcsMask.Hash;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
       public  void Reset () {
            IncludeCount = 0;
            ExcludeCount = 0;
            Hash = 0;
       }
       
#if UNITY_2020_3_OR_NEWER
       [UnityEngine.Scripting.Preserve]
#endif
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public EcsMask Inc<T> () where T : struct 
        {
            var poolId = _world.GetPool<T>().GetId();
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
            if (_built) { throw new Exception ("Cant change built mask.. type = " + typeof(T) + " poolId = " + poolId); }
            if (Array.IndexOf (Include, poolId, 0, IncludeCount) != -1) { throw new Exception ($"{typeof (T).Name} already in constraints list."); }
            if (Array.IndexOf (Exclude, poolId, 0, ExcludeCount) != -1) { throw new Exception ($"{typeof (T).Name} already in constraints list."); }
#endif
            if (IncludeCount == Include.Length)
            {
                Array.Resize (ref Include, IncludeCount << 1);
            }
            Include[IncludeCount++] = poolId;
            return this;
        }

        #if UNITY_2020_3_OR_NEWER
        [UnityEngine.Scripting.Preserve]
        #endif
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public EcsMask Exc<T> () where T : struct 
        {
            var poolId = _world.GetPool<T> ().GetId ();
            if (ExcludeCount == Exclude.Length)
            {
                Array.Resize (ref Exclude, ExcludeCount << 1);
            }
            Exclude[ExcludeCount++] = poolId;
            return this;
        }

#if UNITY_2020_3_OR_NEWER
        [UnityEngine.Scripting.Preserve]
#endif
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public EcsFilter End(int capacity = 512) 
        {
            EcsTools.InsertionSort (Include,  IncludeCount);
            EcsTools.InsertionSort (Exclude,  ExcludeCount);
            Hash = IncludeCount + ExcludeCount;
            for (int i = 0, iMax = IncludeCount; i < iMax; i++) 
            {
                Hash = unchecked (Hash * 314159 + Include[i]);
            }
            for (int i = 0, iMax = ExcludeCount; i < iMax; i++) 
            {
                Hash = unchecked (Hash * 314159 - Exclude[i]);
            }
            var (filter, isNew) = _world.GetFilterInternal (this, capacity);
            if (!isNew)
            {
                _world.Recycle(this);
            }
            return filter;
        }
    }
}