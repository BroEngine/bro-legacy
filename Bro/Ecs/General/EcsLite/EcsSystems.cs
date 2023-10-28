// ----------------------------------------------------------------------------
// The MIT License
// Lightweight ECS framework https://github.com/Leopotam/ecslite
// Copyright (c) 2021-2022 Leopotam <leopotam@gmail.com>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Bro;
using Bro.Ecs;

namespace Leopotam.EcsLite {
    public interface IEcsSystem { }

    public interface IEcsPreInitSystem : IEcsSystem {
        void PreInit (EcsSystems systems);
    }

    public interface IEcsInitSystem : IEcsSystem {
        void Init (EcsSystems systems);
    }

    public interface IEcsRunSystem : IEcsSystem {
        void Run (EcsSystems systems);
    }

    public interface IEcsDestroySystem : IEcsSystem {
        void Destroy (EcsSystems systems);
    }

    public interface IEcsPostDestroySystem : IEcsSystem {
        void PostDestroy (EcsSystems systems);
    }

    public class EcsSystems
    {
        private bool _isInitialized;
        private bool _isPreInitialized;
        private bool _isStopped;
 
        EcsWorld _defaultWorld;
        private int _iterationIndex;
        private readonly EventSystem _eventSystem = new EventSystem();
        readonly Dictionary<string, EcsWorld> _worlds;
        readonly List<IEcsSystem> _allSystems;
        readonly Dictionary<Type, object> _shared = new Dictionary<Type, object>();
        IEcsRunSystem[] _runSystems;
        int _runSystemsCount;
        
        public bool IsLogEnabled = false;
        public bool IsInitialized => _isInitialized;

        public EcsSystems (EcsWorld defaultWorld) {
            _defaultWorld = defaultWorld;
            _worlds = new Dictionary<string, EcsWorld> (32);
            _allSystems = new List<IEcsSystem> (128);
        }

        public Dictionary<string, EcsWorld> GetAllNamedWorlds () {
            return _worlds;
        }

        public string GetWorldName(EcsWorld world, string defaultName = null)
        {
            foreach (var item in _worlds)
            {
                if (item.Value == world)
                {
                    return item.Key;
                }
            }

            return defaultName;
        }

        public int IterationIndex => _iterationIndex;
        
        public int GetAllSystems (ref IEcsSystem[] list) {
            var itemsCount = _allSystems.Count;
            if (itemsCount == 0) { return 0; }
            if (list == null || list.Length < itemsCount) {
                list = new IEcsSystem[_allSystems.Capacity];
            }
            for (int i = 0, iMax = itemsCount; i < iMax; i++) {
                list[i] = _allSystems[i];
            }
            return itemsCount;
        }  
        
        public T GetSystem<T> () where T : IEcsRunSystem // Удалить! как VisibilityObjectSystem приведете в порядок
        {
            var targetType = typeof(T);
            var itemsCount = _allSystems.Count;
            
            for (int i = 0, iMax = itemsCount; i < iMax; i++)
            {
                var system = _allSystems[i];
                if (system.GetType() == targetType)
                {
                    return (T) system;
                }
            }

            throw new Exception();
        }

        public int GetRunSystems (ref IEcsRunSystem[] list) {
            var itemsCount = _runSystemsCount;
            if (itemsCount == 0) { return 0; }
            if (list == null || list.Length < itemsCount) {
                list = new IEcsRunSystem[_runSystems.Length];
            }
            for (int i = 0, iMax = itemsCount; i < iMax; i++) {
                list[i] = _runSystems[i];
            }
            return itemsCount;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public T GetShared<T> () where T : class
        {
            var type = typeof(T);
            if (_shared.ContainsKey(type))
            {
                return _shared[type] as T;
            }
            Bro.Log.Error("shared object with type = " + type + " not exist");
            return null;
        } 
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetShared<T> (T o)
        {
            SetShared(typeof(T), o);
        }  
        
        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void SetShared(Type type, object o) 
        {
            _shared[type] = o;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public void ClearShared()
        {
            _shared.Clear();
        }
        
        public Dictionary<Type, object> GetShared()
        {
            return _shared;
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public EcsWorld GetWorld (string name = null) {
            if (string.IsNullOrEmpty(name)) {
                return _defaultWorld;
            }
            _worlds.TryGetValue (name, out var world);
            return world;
        }

        public void Destroy () {
            for (var i = _allSystems.Count - 1; i >= 0; i--) {
                if (_allSystems[i] is IEcsDestroySystem destroySystem) {
                    destroySystem.Destroy (this);
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                    var worldName = CheckForLeakedEntities ();
                    if (worldName != null) { throw new System.Exception ($"Empty entity detected in world \"{worldName}\" after {destroySystem.GetType ().Name}.Destroy()."); }
#endif
                }
            }
            for (var i = _allSystems.Count - 1; i >= 0; i--) {
                if (_allSystems[i] is IEcsPostDestroySystem postDestroySystem) {
                    postDestroySystem.PostDestroy (this);
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                    var worldName = CheckForLeakedEntities ();
                    if (worldName != null) { throw new System.Exception ($"Empty entity detected in world \"{worldName}\" after {postDestroySystem.GetType ().Name}.PostDestroy()."); }
#endif
                }
            }
            _allSystems.Clear ();
            _runSystems = null;
        }

        public EcsSystems AddWorld (EcsWorld world, string name)
        {
            if (!_worlds.ContainsKey(name))
            {
                _worlds[name] = new EcsWorld();
            }
            
            _worlds[name] = world;
            
            return this;
        }
        
        public EcsSystems AddWorld (EcsWorld world)
        {
            if (_defaultWorld == null)
            {
                _defaultWorld = new EcsWorld();
            }
            // todo findme remove
            // _defaultWorld = world ; // должно быть так
            _defaultWorld.Set(world); // а не так
            // хотя фильтры же
            
            return this;
        }

        public void Add(IEcsSystem system, bool condition) // проба
        {
            if (condition)
            {
                Add(system);
            }
        }

        public EcsSystems Add (IEcsSystem system) {
            _allSystems.Add (system);
            if (system is IEcsRunSystem) {
                _runSystemsCount++;
            }
            return this;
        }

        public void Stop()
        {
            _isStopped = true;
        }

        public void PreInit()
        {
            if (!_isPreInitialized)
            {
                foreach (var system in _allSystems) 
                {
                    if (system is IEcsPreInitSystem initSystem) 
                    {
                        initSystem.PreInit (this);
                    }
                }
                _isPreInitialized = true;
            }
        }

        public void Init()
        {
            PreInit();

            _isInitialized = true;
            
            if (_runSystemsCount > 0) 
            {
                _runSystems = new IEcsRunSystem[_runSystemsCount];
            }
            
            var runIdx = 0;
            foreach (var system in _allSystems) 
            {
                if (system is IEcsInitSystem initSystem) 
                {
                    initSystem.Init (this);
                }
                if (system is IEcsRunSystem runSystem) 
                {
                    _runSystems[runIdx++] = runSystem;
                }
            }
        }

        
        public void Run(int iterationIndex)
        {
            _iterationIndex = iterationIndex;

            // Profiler.BeginSample("ecs run");
            Run();
            // Profiler.EndSample();
        }

        public void Run()
        {
            Bro.Log.Assert(_isInitialized, "systems have to be initialized before run");

            if (_isStopped)
            {
                return;
            }

            for (int i = 0, iMax = _runSystemsCount; i < iMax; i++) 
            {
                
                // Profiler.BeginSample("ecs system: " + _runSystems[i].GetType());
                _runSystems[i].Run (this);
                // Profiler.EndSample();
                
#if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
                var worldName = CheckForLeakedEntities ();
                if (worldName != null) { throw new System.Exception ($"Empty entity detected in world \"{worldName}\" after {_runSystems[i].GetType ().Name}.Run()."); }
#endif
            }
            
            _eventSystem.Run(this);
            
            ++_iterationIndex;
        }

        #if ECS_DEBUG && !LEOECSLITE_NO_SANITIZE_CHECKS
        public string CheckForLeakedEntities () {
            if (_defaultWorld.CheckForLeakedEntities ()) { return "default"; }
            foreach (var pair in _worlds) {
                if (pair.Value.CheckForLeakedEntities ()) {
                    return pair.Key;
                }
            }
            return null;
        }
        #endif
    }
}