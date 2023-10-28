using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Bro.Fossil;
using Bro.Network;
using Bro.Server.Context;
using Bro.Server.Network;
using Bro.Sketch.Network;

namespace Bro.Ecs
{
    public class WorldReporter
    {
        private const int MaxDeltaFrame = 100;
        private const int MaxDeltaSize = 1024;  // mtu 1060 
        
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
        
        private int _lastReportedFrame;
        private readonly ServerFrameController _frameController;
        private readonly ServerContext _context;

        private readonly List<IClientPeer> _sharedApplied = new List<IClientPeer>();
        private readonly List<IClientPeer> _referenceApplied = new List<IClientPeer>();
        
        public WorldReporter(ServerContext context, ServerFrameController frameController)
        {
            _context = context;
            _frameController = frameController;
            _context.AttachHandlers(this);
        }

        public void Report()
        {
            var currentFrame = _frameController.Frame;
            if (_lastReportedFrame == currentFrame)
            {
                return;
            }
            
            _lastReportedFrame = currentFrame;
            
            var historyFrame = currentFrame - 1;
            var historyWorld = _frameController.GetHistory(historyFrame);
            
            if (historyWorld != null)
            {
                var framesDelta = historyFrame - _referenceFrame;
                var isReference = framesDelta >= MaxDeltaFrame || !_isReferenceSet;
                
                if (!isReference && _isReferenceSet)
                {
                    _deltaFrameSize = _serializer.SerializeWorld(historyWorld, _deltaFrameBuffer);
                    _deltaDataSize = _delta.Create(_referenceFrameBuffer, _referenceFrameSize, _deltaFrameBuffer, _deltaFrameSize, _deltaDataBuffer );

                    if (_deltaDataSize > MaxDeltaSize) /* mtu fixed sized delta */
                    {
                        isReference = true;
                    }
                }
                
                if (isReference)
                {
                    _isReferenceSet = true;
                    
                    _referenceFrame = historyFrame;
                    _referenceFrameSize = _serializer.SerializeWorld(historyWorld, _referenceFrameBuffer);

                    _deltaFrameSize = 0;
                    _deltaDataSize = 0;
                    
                    Send(historyFrame, _referenceFrame, _referenceFrameBuffer, _referenceFrameSize, true);
                }
                else if (_isReferenceSet)
                {
                    Send(historyFrame, _referenceFrame, _deltaDataBuffer, _deltaDataSize, false);
                }
            }
        }

        private void Send(int frame, int referenceFrame, byte[] data, int size, bool reliable)
        {
            var worldEvent = NetworkPool.GetOperation<GameWorldEvent>();
            worldEvent.IsReliable = reliable;
            worldEvent.ReferenceFrame.Value = referenceFrame;
            worldEvent.Frame.Value = frame;
            worldEvent.Data.Set(data, size);
            
            _context.ForEachPeer((peer) =>
            {
                if (!IsSharedApplied(peer))
                {
                    //temp
                    _sharedApplied.Remove(peer);
                    _sharedApplied.Add(peer);
                    
                    worldEvent.Shared.Value = _frameController.GetSharedAsJson();
                }
                else
                {
                    worldEvent.Shared.Cleanup();
                }

                if (IsReferenceApplied(peer))
                {
                    peer.Send(worldEvent);
                }
                else
                {
                    Bro.Log.Error("send frame = " + frame + "; ref frame = " + referenceFrame + "; size = " + size );
                    SendReference(peer);
                }
            });
        }

        private void SendReference(IClientPeer peer)
        {
            Bro.Log.Error("reference frame send to peer id = " + peer.PeerId);
            var worldEvent = NetworkPool.GetOperation<GameWorldEvent>();
            worldEvent.ReferenceFrame.Value = _referenceFrame;
            worldEvent.Frame.Value = _referenceFrame;
            worldEvent.Data.Set(_referenceFrameBuffer, _referenceFrameSize);
            worldEvent.Shared.Value = _frameController.GetSharedAsJson();
            peer.Send(worldEvent);
            _referenceApplied.Add(peer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsSharedApplied(IClientPeer peer)
        {
            return _sharedApplied.Contains(peer);
        }    
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsReferenceApplied(IClientPeer peer)
        {
            return _referenceApplied.Contains(peer);
        }

        [PeerJoinedHandler]
        private void OnPeerJoined(IClientPeer peer, object data)
        {

        }

        [PeerLeftHandler]
        private void OnPeerLeft(IClientPeer peer)
        {
            _sharedApplied.Remove(peer);
            _referenceApplied.Remove(peer);
        }
        
        [RequestHandler(Request.OperationCode.EcsWorld)]
        private INetworkResponse OnWorldRequest(INetworkRequest request, IClientPeer peer)
        {
            var worldRequest = (GameWorldRequest) request;

            if (worldRequest.SharedApplied.IsInitialized)
            {
                if (worldRequest.SharedApplied.Value)
                {
                    _sharedApplied.Remove(peer);
                    _sharedApplied.Add(peer);
                }
            }
            return null;
        }
    }
}