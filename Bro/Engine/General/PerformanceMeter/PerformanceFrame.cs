using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Bro
{
    public class PerformanceFrame
    {
        private static int _frameCounter;
        
        private class Accumulation
        {
            public readonly Enum Enum;
            public int DoneCount;
            public int FailedCount;
            public long TotalTime;
            public long MaxTime;
            public long MinTime = -1;
            
            public Accumulation(Enum e)
            {
                Enum = e;
            }
        }

        private readonly Dictionary<string,Accumulation> _data = new Dictionary<string, Accumulation>();

        public readonly int FrameId;
        
        public PerformanceFrame()
        {
            FrameId = Interlocked.Increment(ref _frameCounter);
        }

        public void AddSample( PerformancePoint point )
        {
            var isDone = point.IsDone;
            var isFailed = point.IsFailed;
            var microtime = point.MicroTime;
            var accumulation = GetAccumulation(point.Enum, point.Key);

            if (isFailed)
            {
                accumulation.FailedCount++;
            }
            else
            {
                accumulation.DoneCount++;
            }

            accumulation.TotalTime += microtime;
            
            if ( microtime > accumulation.MaxTime )
            {
                accumulation.MaxTime = microtime;
            }

            if (microtime < accumulation.MinTime || accumulation.MinTime == -1)
            {
                accumulation.MinTime = microtime;
            }
        }

        private Accumulation GetAccumulation(Enum e, string key)
        {
            if (! _data.ContainsKey(key))
            {
                _data[key] = new Accumulation(e);
            }
            
            return  _data[key];
        }

        public void PrintResult()
        {
            Bro.Log.Performance("performance meter :: frame index = " + FrameId + " accumulation info ###########################################################################" );
            
            var sorted = from pair in _data orderby Convert.ToInt32( pair.Value.Enum ) ascending select pair;

            foreach (var dataPair in sorted)
            {
                var name = dataPair.Key;
                var accumulation = dataPair.Value;

                var calls = accumulation.FailedCount + accumulation.DoneCount;
                var total = accumulation.TotalTime / 1000.0f;
                var min = accumulation.MinTime / 1000.0f;
                var max = accumulation.MaxTime / 1000.0f;
                var avg = calls != 0 ? total / calls : 0;
                var fails = accumulation.FailedCount;
                
                Bro.Log.Performance( string.Format( "performance meter :: {0} : calls = {1}; total = {2}ms; min = {3}ms; max = {4}ms; avg = {5}ms; fails = {6}", name, calls, total, min, max, avg, fails ) );
            }
            
            Bro.Log.Performance("performance meter :: frame end ####################################################################################################" );
        }
    }
}