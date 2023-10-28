using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Bro
{
    public static class TimeInfo
    {
        public const long Day = 86400000;
        public const long Week = Day * 7;

        private static long _applicationStartTimestamp;
        private static Stopwatch _applicationStartTimer;

        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private static double _timeOffset;

        public static long TimeOffset => (long) _timeOffset;

        public static long GlobalTimestamp => LocalTimestamp + (long) _timeOffset;
        
        public static long LocalTimestamp
        {
            get
            {
                if (_applicationStartTimestamp == 0)
                {
                    _applicationStartTimestamp = (long) (DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
                    _applicationStartTimer = new Stopwatch();
                    _applicationStartTimer.Start();
                }

                return _applicationStartTimestamp + _applicationStartTimer.ElapsedMilliseconds;
            }
        }

        public static long GlobalToLocalTimestamp(long globalTime) => globalTime - (long) _timeOffset;

        /// <summary>
        /// Convert to local non unix time 
        /// </summary>
        public static long LocalTimestampToLocalSeconds(long localTimestamp)
        {
            var dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds( localTimestamp ).ToLocalTime();;
            return (long)dtDateTime.TimeOfDay.TotalMilliseconds;
        }

        public static long LocalMidnightTimestamp(int daysBefore)
        {
            TimeSpan timeShift = new TimeSpan(daysBefore, 0, 0, 0);
            return (long) (DateTime.UtcNow.Date - timeShift - UnixEpoch).TotalMilliseconds;
        }

        public static long GlobalMidnightTimestamp(int daysBefore)
        {
            return LocalMidnightTimestamp(daysBefore) + (long) _timeOffset;
        }

        public static long LocalLastMidnightTimestamp => (long) (DateTime.UtcNow.Date - UnixEpoch).TotalMilliseconds;

        public static long GlobalLastMidnightTimestamp => LocalLastMidnightTimestamp + (long) _timeOffset;

        #region Time Sync

        private static readonly LinkedList<double> _samples = new LinkedList<double>();
        private static readonly int _minSamplesAmount = 50;
        private static readonly int _samplesRecalcAmount = 100;

        private static bool _hasTimeOffsetBoundaries = false;
        private static double _minTimeOffset = long.MinValue;
        private static double _maxTimeOffset = long.MaxValue;

        public static void SetTimeSyncBoundaries(long minGlobalTimestamp, long maxGlobalTimestamp)
        {
            var localTimestamp = LocalTimestamp;
            _minTimeOffset = minGlobalTimestamp - localTimestamp;
            _maxTimeOffset = maxGlobalTimestamp - localTimestamp;
            _hasTimeOffsetBoundaries = true;
            Bro.Log.Assert(_minTimeOffset<_maxTimeOffset, "invalid time boundaries");
            CheckTimeOffsetBoundaries();
        }

        public static void ResetTimeSyncBoundaries()
        {
            _hasTimeOffsetBoundaries = false;
            _minTimeOffset = long.MinValue;
            _maxTimeOffset = long.MaxValue;
        }

        private static void CheckTimeOffsetBoundaries()
        {
            if (_hasTimeOffsetBoundaries)
            {
                _timeOffset = Math.Max(_minTimeOffset, _timeOffset);
                _timeOffset = Math.Min(_maxTimeOffset, _timeOffset);
            }
        }

        private static void AddTimeOffset(double newTimeOffset)
        {
            InsertSample(newTimeOffset);
            RemoveUnusedSamples();

            var samplesCount = _samples.Count;
            if (samplesCount < _minSamplesAmount)
            {
                _timeOffset = 0f;
                foreach (var s in _samples)
                {
                    _timeOffset += s;
                }

                _timeOffset /= ((double) _samples.Count);
            }
            else
            {
                var sideOffset = (samplesCount - _minSamplesAmount) / 2;
                var fromIndex = sideOffset;
                var toIndex = _minSamplesAmount + sideOffset;
                var counter = 0;
                _timeOffset = 0f;
                foreach (var s in _samples)
                {
                    if (counter >= fromIndex)
                    {
                        if (counter >= toIndex)
                        {
                            break;
                        }
                        else
                        {
                            _timeOffset += s;
                        }
                    }

                    ++counter;
                }

                _timeOffset /= ((double) _minSamplesAmount);
            }
            CheckTimeOffsetBoundaries();
        }

        private static void RemoveUnusedSamples()
        {
            var samplesCount = _samples.Count;
            if (samplesCount >= _samplesRecalcAmount)
            {
                var removeCount = samplesCount - _minSamplesAmount;
                while (removeCount > 0)
                {
                    --removeCount;
                    if (removeCount % 2 == 0)
                    {
                        _samples.RemoveFirst();
                    }
                    else
                    {
                        _samples.RemoveLast();
                    }
                }
            }
        }

        private static void InsertSample(double newTimeOffset)
        {
            var current = _samples.First;
            if (current == null)
            {
                _samples.AddFirst(newTimeOffset);
            }
            else
            {
                while (true)
                {
                    if (current.Value >= newTimeOffset)
                    {
                        _samples.AddBefore(current, newTimeOffset);
                        break;
                    }

                    current = current.Next;
                    if (current == null)
                    {
                        _samples.AddLast(newTimeOffset);
                        break;
                    }
                }
            }
        }

        public static void ResetTimeSync(bool hard)
        {
            ResetTimeSyncBoundaries();
            
            if (hard)
            {
                _timeOffset = 0.0f;
            }

            _samples.Clear();
            
        }


        public static void SyncTime(long serverTimestamp, long pingTimestamp)
        {
            var currentServerTimestamp = serverTimestamp + pingTimestamp;
            var newTimeOffset = (double) (currentServerTimestamp - LocalTimestamp);
            AddTimeOffset(newTimeOffset);
        }

        #endregion
    }
}