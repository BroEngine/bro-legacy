using System;
using System.Collections.Generic;
using Bro.Ecs.Network;
using Leopotam.EcsLite;

namespace Bro.Ecs
{
    public abstract class FrameController : FrameUpdater
    {   
        public const string ImpactWorld = "impact";
        
        public delegate void SharedDataDelegate(Dictionary<Type, object> sharedData);
        public event SharedDataDelegate OnSharedDataChanged;

        protected int _frame;
        protected int _deltaFrame;
        protected readonly long _frameInterval;
        protected readonly int _frameCapacity;
      
        protected readonly SharedController _shared = new SharedController();
        private readonly Dictionary<int, GameWorldFrame> _impactFrames = new Dictionary<int, GameWorldFrame>();

        public int Frame => _frame;
        public int DeltaFrame => _deltaFrame;
        
        protected FrameController(int frames, long frameInterval) : base(frameInterval)
        {
            _frameInterval = frameInterval;
            _frameCapacity = frames;

            for (int i = 0, iMax = _frameCapacity; i < iMax; i++)
            {
                _impactFrames[i] = new GameWorldFrame(new EcsWorld(),-1);
            }
        }
        
        protected override void OnFrame()
        {
            throw new NotImplementedException();
        }

        public EcsWorld GetImpactWorld(int frame)
        {
            if (_impactFrames[frame % _frameCapacity].Frame != frame)
            {
                _impactFrames[frame % _frameCapacity].World.Clear();
                _impactFrames[frame % _frameCapacity].Frame = frame;
            }
            return _impactFrames[frame % _frameCapacity].World;
        }
        
        public void SetShared<T>(object o)
        {
            if (IsRunning)
            {
                Bro.Log.Error("shared object can only be added before simulation started");
                return;
            }
            
            _shared.SetShared<T>(o);
        }

        public string GetSharedAsJson()
        {
            return _shared.GetSharedAsJson();
        }

        public void SetSharedFromJson(string json)
        {
            if (IsRunning)
            {
                Bro.Log.Error("shared object can only be added before simulation started");
                return;
            }
            OnSharedDataChanged?.Invoke(_shared.SetSharedFromJson(json));
        }

        public abstract void AddSystem(IEcsSystem system);
        public abstract void AddSystem(IEcsSystem system, bool condition);
    }
}