#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII || CONSOLE_CLIENT

using System;
using Bro.Client;
using Bro.Client.Network;
using Bro.Ecs.Debug;
using Bro.Fossil;
using Bro.Sketch.Client;
using Bro.Toolbox.Client;
using Leopotam.EcsLite;

namespace Bro.Ecs
{
    public class WorldSynchronizer : IDisposable
    {
        private readonly WorldSerializer _serializer = new WorldSerializer();
        private readonly Delta _delta = new Delta();
        
        private int _referenceFrame;
        private bool _isReferenceSet;
        
        private readonly byte[] _referenceFrameBuffer = new byte[Delta.BufferSize];
        private int _referenceFrameSize;

        private readonly byte[] _deltaFrameBuffer = new byte[Delta.BufferSize];
        private int _deltaFrameSize;
        
        private readonly byte[] _deltaDataBuffer = new byte[Delta.BufferSize];
        private int _deltaDataSize;

        private readonly IClientContext _context;
        private readonly NetworkEngine _engine;
        private readonly ClientFrameController _frameController;
        
        private readonly EcsWorld _world = new EcsWorld();
        
        private GameWorldEvent _pendingEvent;
        private int _pendingFrame = -1;
        private bool _pendingIsReference;

        private readonly DebugDropper _debugDropper = new DebugDropper();
     
        public WorldSynchronizer(IClientContext context, ClientFrameController frameController)
        {
            _context = context;
            _engine = _context.GetNetworkEngine();
            _frameController = frameController;
            _context.AddDisposable(new NetworkEventObserver<GameWorldEvent>(OnWorldEvent, _engine));
            _context.AddDisposable(_context.Scheduler.ScheduleUpdate(OnUpdate));
        }
        
        private void OnWorldEvent(GameWorldEvent e)
        {
            // handle share
            if (e.Shared.IsInitialized)
            {
                if (!_frameController.IsRunning)
                {
                    _frameController.SetSharedFromJson(e.Shared.Value);   
                }
                ConfirmShared();
            }
            
            // save pending
            var frame = e.Frame.Value;
            var referenceFrame = e.ReferenceFrame.Value;
            var isReference = frame == referenceFrame;

            if ((_debugDropper.IsDrop() && ! isReference) || _debugDropper.IsAllDrop())
            {
                return;
            }
            
            EcsNetDebug.Instance.SetLastNetworkFrame(frame);
            EcsNetDebug.Instance.SetWorldEventNetworkSize(e.Data.Size);
            
            if (frame > _pendingFrame)
            {
                if (!_pendingIsReference || (_pendingIsReference && isReference))
                {
                    _pendingEvent?.Release();
                    _pendingFrame = frame;
                    _pendingIsReference = isReference;
                    _pendingEvent = e;
                    e.Retain();
                }
                else
                {
                    EcsNetDebug.Instance.SetLastNetworkFrameDropped(frame);
                }
            }
            else
            {
                EcsNetDebug.Instance.SetLastNetworkFrameDropped(frame);
            }
        }

        private void OnUpdate(float dt)
        {
            if (_pendingEvent != null)
            {
                HandleEvent(_pendingEvent);
                _pendingEvent.Release();
                _pendingEvent = null;
                _pendingIsReference = false;
            }
        }
        
        private void HandleEvent(GameWorldEvent e)
        {
            var frame = e.Frame.Value;
            var referenceFrame = e.ReferenceFrame.Value;
            var isReference = frame == referenceFrame;
            
            EcsNetDebug.Instance.SetLastNetworkFrameHandled(frame);
            
            if (isReference)
            {
                _isReferenceSet = true;
                _referenceFrame = referenceFrame;
                _referenceFrameSize = e.Data.Get(_referenceFrameBuffer);
                HandleFrame(frame, _referenceFrameBuffer, _referenceFrameSize);
            }
            else
            {
                if (_isReferenceSet && _referenceFrame == referenceFrame)
                {
                    _deltaDataSize = e.Data.Get(_deltaDataBuffer);
                    _deltaFrameSize = _delta.Apply(_referenceFrameBuffer, _referenceFrameSize, _deltaDataBuffer, _deltaDataSize, _deltaFrameBuffer);
                    HandleFrame(frame, _deltaFrameBuffer, _deltaFrameSize);
                }
                else
                {
                    Bro.Log.Error("shit " +_isReferenceSet + " " + _referenceFrame + " / " + referenceFrame );
                }
            }
        }

        private void HandleFrame(int frame, byte[] data, int size)
        {
            var rtt = _engine.Rtt;
            _world.Clear();
            _serializer.DeserializeWorld(_world, data, size);
            _frameController.ApplyFrame(_world, frame, rtt);
            
            var referenceDelta = _frameController.GetReferenceDelta(rtt);
            EcsNetDebug.Instance.UpdateRtt(rtt,referenceDelta);
        }

        private void ConfirmShared()
        {
            var request = NetworkPool.GetOperation<GameWorldRequest>();
            request.SharedApplied.Value = true;
            new SendRequestTask(request, _engine).Launch(_context);
        }

        public void SetDebugDropFactor(float factor)
        {
            _debugDropper.SetFactor(factor);
        }
        public void Dispose()
        {
            _serializer?.Dispose();
        }
    }
}

#endif