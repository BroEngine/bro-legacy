// ReSharper disable InconsistentNaming

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Bro.Threading;

namespace Bro
{
    public static class PerformanceMeter
    {
        private static volatile bool _enabled = false;
        private static int _frameSize = 0;
        
        private static readonly List<PerformancePoint> _processingPoints = new List<PerformancePoint>();
        private static readonly List<PerformancePoint> _pendingPoints = new List<PerformancePoint>();
        private static readonly List<PerformancePoint> _returningPoints = new List<PerformancePoint>();
        private static List<Enum> _mask = new List<Enum>();
        private static readonly object _lock = new object();

        private static BroThread _thread;

        public static bool Enabled { get { return _enabled; } }
        public static int FrameSize { get { return _frameSize; } }
        
        public static void Configurate( bool enabled, int frameSize = 5000, List<Enum> mask = null )
        {
            Bro.Log.Info("performance meter :: configured, enabled = " + enabled + ", frame size = " + frameSize);

            _mask = mask;
            _enabled = frameSize != 0 && enabled;
            Interlocked.Exchange(ref _frameSize, frameSize);

            if (!_enabled && _thread != null)
            {
                _thread.Join();
                _thread = null;
            }

            if (_enabled && _thread == null)
            {
                _thread = new BroThread(WorkCycle);
                _thread.Start();
            }
        }

        public static PerformancePoint Register(Enum type, string data = null)
        {
            if (!_enabled)
            {
                return null;
            }

            if (!IsAvailable(type))
            {
                return null;
            }

            lock (_lock)
            {
                var point = new PerformancePoint(type, data);
                _pendingPoints.Add(point);
                return point;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IsAvailable(Enum type)
        {
            if (_mask != null && _mask.Count > 0)
            {
                foreach (var mask in _mask)
                {
                    if (Equals(mask, type))
                    {
                        return true;
                    }
                }
                return false;
            }
            return true;
        }

        private static void WorkCycle()
        {
            Bro.Log.Info("performance meter :: frame thread started");
            while (_enabled)
            {
                ProcessFrame();
                Thread.Sleep(_frameSize);
            }
        }
        
        private static void ProcessFrame()
        {
            lock (_lock)
            {
                _processingPoints.AddRange(_pendingPoints);
                _pendingPoints.Clear();
            }

            /* process */

            var frame = new PerformanceFrame();
            
            foreach (var point in _processingPoints)
            {
                var isDone = point.IsDone;
                var isFailed = point.IsFailed;

                if (!isDone && !isFailed)
                {
                    _returningPoints.Add(point);
                    continue;
                }
                frame.AddSample( point );
            }

            frame.PrintResult();
            
            _processingPoints.Clear();
            /* process */
            
            lock (_lock)
            {
                _pendingPoints.AddRange( _returningPoints );
            }
            
            _returningPoints.Clear();
        }
    }
}