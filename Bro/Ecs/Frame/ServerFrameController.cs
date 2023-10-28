using System;
using System.Collections.Generic;
using Bro.Ecs.Network;
using Leopotam.EcsLite;

namespace Bro.Ecs
{
    public class ServerFrameController : FrameController, IDisposable
    {
        private readonly EcsSystems _systems;
        private readonly Dictionary<int, GameWorldFrame> _history = new Dictionary<int, GameWorldFrame>();

        public ServerFrameController(int frames, long frameInterval) : base(frames, frameInterval)
        {
            _systems = new EcsSystems(null);
            _systems.IsLogEnabled = true;
            _shared.AddSystems(_systems);
            
            for (int i = 0, iMax = _frameCapacity; i < iMax; i++)
            {
                _history[i] = new GameWorldFrame(new EcsWorld(),-1);
            }
        }
        
        public EcsWorld GetReplicaWorld()
        {
            return _systems.GetWorld();
        }  
        
        public EcsWorld GetHistory(int frame)
        {
            if (_history[frame % _frameCapacity].Frame != -1)
            {
                return _history[frame % _frameCapacity].World;
            }
            return null;
        }
        
        public override void Start()
        {
            if (_systems.GetWorld() == null)
            {
                Bro.Log.Info("create world");
                _systems.AddWorld(new EcsWorld());
            }
            _systems.Init();
            base.Start();
        }
        
        protected override void OnFrame()
        {
            Simulate(_frame);
            ++_frame;
        }

        private void Simulate(int frame)
        {
            _systems.AddWorld(GetImpactWorld(frame), ImpactWorld);
            _systems.Run(frame);
            
            _history[frame % _frameCapacity].World.Set(_systems.GetWorld());
            _history[frame % _frameCapacity].Frame = frame;
        }
        
        public override void AddSystem(IEcsSystem system)
        {
            _systems.Add(system);
        } 
        
        public override void AddSystem(IEcsSystem system, bool condition) 
        {
            if (condition)
            {
                AddSystem(system);
            }
        }

        public void Dispose()
        {
            _systems.Destroy();
        }
    }
}