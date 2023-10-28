using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsLite;

namespace Bro.Ecs
{
    public class ClientFrameController : FrameController, IDisposable
    {
        public delegate void FrameDelegate(int frame);

        public event FrameDelegate OnBeforeFrame; 
        public event FrameDelegate OnAfterFrame; 
     
        public bool IsOwnerSimulationEnable { get; private set; }
        
        private readonly short _ownerId;
        private readonly OwnerCopier _ownerCopier;
        private readonly EcsWorldComparer _comparer;
        private readonly EcsSystems _ownersSystems;
        private readonly EcsSystems _replicaSystems;

        private readonly EcsWorld _ownerWorld;
        
        public ClientFrameController(short ownerId, int frames, long frameInterval) : base(frames, frameInterval)
        {
            // System.IO.Compression.GZipStream
            _ownerId = ownerId;
            _comparer = new EcsWorldComparer();
            _ownerCopier = new OwnerCopier();
            _ownerWorld = new EcsWorld();
            _ownersSystems = new EcsSystems(_ownerWorld);
            _replicaSystems = new EcsSystems(null);
            _shared.AddSystems(_ownersSystems);
            _shared.AddSystems(_replicaSystems);
            IsOwnerSimulationEnable = true;
        }
        
        public EcsWorld GetReplicaWorld()
        {
            return _replicaSystems.GetWorld();
        }

        protected override void OnFrame()
        {
            OnBeforeFrame?.Invoke(_frame);
            
            var replicaWorld = _replicaSystems.GetWorld();
            var replicateFrame = replicaWorld.GetIterationIndex() + 1;
            Simulate(_replicaSystems, replicateFrame, false); // тут по идее вообще импакт применять не нужно
            
            if (IsOwnerSimulationEnable)
            {
                Simulate(_ownersSystems, _frame, true);
                var ownerWorld = _ownersSystems.GetWorld();
                _ownerCopier.CopyComponents(ownerWorld, replicaWorld, _ownerId);
            }

            ++_frame;
            
            OnAfterFrame?.Invoke(_frame);
        }

        private void Simulate(EcsSystems systems, int frame, bool applyImpact = true)
        {
            if (applyImpact)
            {
                systems.AddWorld(GetImpactWorld(frame), ImpactWorld);
            }
            else
            {
                systems.AddWorld(null, ImpactWorld);
            }

            systems.Run(frame);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetDelta(int serverFrame)
        {
            return _frame - serverFrame;
        }

        private void GoAhead(int serverFrame, int referenceDelta) /* догоняем (обгоняем) сервер */
        {
            if (GetDelta(serverFrame) < referenceDelta)
            {
                while (GetDelta(serverFrame) < referenceDelta)
                {
                    OnFrame();
                }
                Bro.Log.Assert(GetDelta(serverFrame) >= referenceDelta, "actual delta " + (_frame - serverFrame) + " < reference delta " + referenceDelta);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetReferenceDelta(long rtt)
        {
            var halfRttTime = rtt / 2.0f;
            var halfRttFrames = (int) Math.Ceiling(halfRttTime * 1.0 / _frameInterval);
            var bufferFrames = 3 + 1; // + поллинг внутренних ивентов
            return halfRttFrames + bufferFrames;
        }
        
        public void ApplyFrame(EcsWorld newReplicaWorld, int serverFrame, long rtt)
        {
            var referenceDelta = GetReferenceDelta(rtt);
            var referenceClientFrame = serverFrame + referenceDelta;

            _deltaFrame = referenceDelta;
            
            if (!IsRunning) // initial set
            {
                _frame = referenceClientFrame;
                Start();
                Bro.Log.Info(  $"start frame; client = {_frame}; server = {serverFrame}; delta = {(_frame - serverFrame)}");
            }
            
            Update();
            
            GoAhead(serverFrame, referenceDelta);

            if (IsOwnerSimulationEnable)
            {
                _ownerCopier.CopyComponents(newReplicaWorld, _ownerWorld, _ownerId);
                _ownerCopier.CopyColliders(newReplicaWorld, _ownerWorld);

                var delta = _frame - serverFrame;
                for (var i = delta - 1; i >= 1; --i) // do not simulate current frame
                {
                    var simulationFrame = _frame - i;
                    Simulate(_ownersSystems, simulationFrame);
                }

                _ownerCopier.CopyComponents(_ownersSystems.GetWorld(), newReplicaWorld, _ownerId);
            }

            _replicaSystems.AddWorld(newReplicaWorld);
        }
        
        public override void AddSystem(IEcsSystem system, bool condition) 
        {
            if (condition)
            {
                AddSystem(system);
            }
        }
        
        public override void AddSystem(IEcsSystem system)
        {
            _ownersSystems.Add(system);
            _replicaSystems.Add(system);
        } 
        
        public void AddOwnersSystem(IEcsSystem system)
        {
            _ownersSystems.Add(system);
        }
        
        public void AddReplicateSystem(IEcsSystem system)
        {
            _replicaSystems.Add(system);
        }
        
        public override void Start()
        {
            if (_ownersSystems.GetWorld() == null)
            {
                Bro.Log.Info("create world");
                _ownersSystems.AddWorld(new EcsWorld());
                _replicaSystems.AddWorld(new EcsWorld());
            }
            _ownersSystems.Init();
            _replicaSystems.Init();
            base.Start();
        }

        public void EnableOwnerSimulation(bool enable)
        {
            IsOwnerSimulationEnable = enable;
        }

        public void Dispose()
        {
            _replicaSystems.Destroy();
            _ownersSystems.Destroy();
        }
    }
}