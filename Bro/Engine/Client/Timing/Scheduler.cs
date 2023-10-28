using System;
using System.Collections;
using System.Collections.Generic;

namespace Bro.Client
{
    public partial class Timing
    {
        public class Scheduler : IScheduler
        {
            private readonly IUpdatesProvider _updatesProvider;

    
            public Scheduler(IClientContext clientContext)
            {
                _updatesProvider = clientContext.Application.UpdatesProvider;
             
                _fixedUpdateInterval = _updatesProvider.FixedUpdateInterval;

                _updatesProvider.RegisterUpdate(Update);
                _updatesProvider.RegisterFixedUpdate(FixedUpdate);
                _updatesProvider.RegisterLateUpdate(LateUpdate);
            }

            public void Dispose()
            {
                _updatesProvider.UnregisterUpdate(Update);
                _updatesProvider.UnregisterFixedUpdate(FixedUpdate);
                _updatesProvider.UnregisterLateUpdate(LateUpdate);

                _onUpdate.Clear();
                _onUpdate = null;
                _onLateUpdate = null;
                _onFixedUpdate = null;

                _activeCoroutines.Clear();
            }

            #region Timings
            
            private List<Handler> _onUpdate = new List<Handler>(1024); /* можно прервать все апдейты dispos-ом в середине вызова */
            private event Handler _onLateUpdate;
            private event Handler _onFixedUpdate;

            private readonly List<Handler> _fixedTimingsToRemove = new List<Handler>();
            private readonly List<Handler> _afterFrameTimingsToRemove = new List<Handler>();
            private readonly List<Handler> _frameTimingsToRemove = new List<Handler>();
            
            private readonly float _fixedUpdateInterval;

          

            internal void AddFrameTiming(Base t)
            {
                _onUpdate.Add(t.TimingHandler);
            }

            internal void RemoveFrameTiming(Base t)
            {
                _onUpdate?.Remove(t.TimingHandler);
            }

            internal void AddFixedTiming(Base t)
            {
                _onFixedUpdate += t.TimingHandler;
            }

            internal void RemoveFixedTiming(Base t)
            {
                _fixedTimingsToRemove.Add(t.TimingHandler);
            }

            internal void AddAfterFrameTiming(Base t)
            {
                _onLateUpdate += t.TimingHandler;
            }

            internal void RemoveAfterFrameTiming(Base t)
            {
                _afterFrameTimingsToRemove.Add(t.TimingHandler);
            }

            #endregion

            #region Coroutines

            private readonly List<Coroutine> _activeCoroutines = new List<Coroutine>();
            private readonly List<IEnumerator> _enumeratorsToRemove = new List<IEnumerator>();
            private readonly List<Coroutine> _coroutinesToRemove = new List<Coroutine>();

            public IDisposable Schedule(Action callback, float delay)
            {
                var timer = new HandleAfterDelay(callback, delay, this);
                AddFrameTiming(timer);
                return timer;

            }

            public IDisposable ScheduleFixedUpdate(Handler handler)
            {
                var  timer = new FixedUpdate(handler,this);
                AddFixedTiming(timer);
                return timer;
            }

            public IDisposable ScheduleLateUpdate(Handler handler)
            {
                var timer = new LateUpdate(handler,this);
                AddAfterFrameTiming(timer);
                return timer;

            }

            public IDisposable ScheduleUpdate(Handler handler)
            {
                var timer = new Update(handler, this);
                timer.Start();
                return timer;
            }

            public IDisposable ScheduleEase(Ease.Base ease)
            {
                ease.Setup(this);
                ease.Start();
                return ease;
            }

            public IDisposable StartCoroutine(IEnumerator enumerator)
            {
                var coroutine = new Coroutine(enumerator);
                _activeCoroutines.Add(coroutine);
                return coroutine;
            }

            public void StopCoroutine(Coroutine coroutine)
            {
                if (coroutine == null)
                {
                    return;
                }

                _coroutinesToRemove.Add(coroutine);
            }

            private void StopCoroutine(IEnumerator enumerator)
            {
                if (enumerator == null)
                {
                    return;
                }

                _enumeratorsToRemove.Add(enumerator);
            }

            private void UpdateCoroutines(TickType updateType)
            {
                Coroutine coroutine;
                IEnumerator en;

                // == remove by enumerator
                for (int j = 0, jMax = _enumeratorsToRemove.Count; j < jMax; ++j)
                {
                    en = _enumeratorsToRemove[j];
                    for (int i = 0, iMax = _activeCoroutines.Count; i < iMax; ++i)
                    {
                        if (_activeCoroutines[i].Enumerator == en)
                        {
                            _activeCoroutines.FastRemoveAtIndex(i);
                            break;
                        }
                    }
                }

                _enumeratorsToRemove.Clear();

                // == remove by coroutine object
                for (int j = 0, jMax = _coroutinesToRemove.Count; j < jMax; ++j)
                {
                    _activeCoroutines.Remove(_coroutinesToRemove[j]);
                }

                _coroutinesToRemove.Clear();

                for (int i = 0, max = _activeCoroutines.Count; i < max;)
                {
                    coroutine = _activeCoroutines[i];
                    if (coroutine == null || coroutine.IsCompleted)
                    {
                        _activeCoroutines.FastRemoveAtIndex(i);
                        --max;
                    }
                    else
                    {
                        try
                        {
                            coroutine.Tick(updateType);
                        }
                        catch (Exception exception)
                        {
                            Log.Error(exception);
                        }

                        ++i;
                    }
                }
            }

            #endregion

            #region Updates

            private void Update(float delta)
            {
                for (int i = 0, max = _frameTimingsToRemove.Count; i < max; ++i)
                {
                    _onUpdate.Remove(_frameTimingsToRemove[i]);
                }

                _frameTimingsToRemove.Clear();

                if (_onUpdate == null)
                {
                    return;
                }

                for(var i = 0; i < _onUpdate.Count; ++ i)
                {
                    _onUpdate[i].Invoke(delta);
                    if (_onUpdate == null)
                    {
                        break;
                    }
                }
                
                UpdateCoroutines(TickType.Update);
            }

            private void FixedUpdate(float delta)
            {
                for (int i = 0, max = _fixedTimingsToRemove.Count; i < max; ++i)
                {
                    _onFixedUpdate -= _fixedTimingsToRemove[i];
                }

                _fixedTimingsToRemove.Clear();
                _onFixedUpdate?.Invoke(delta);

                UpdateCoroutines(TickType.FixedUpdate);
            }

            private void LateUpdate(float delta)
            {
                for (int i = 0, max = _afterFrameTimingsToRemove.Count; i < max; ++i)
                {
                    _onLateUpdate -= _afterFrameTimingsToRemove[i];
                }

                _afterFrameTimingsToRemove.Clear();
                _onLateUpdate?.Invoke((int) _fixedUpdateInterval);

                UpdateCoroutines(TickType.LateUpdate);
            }

            #endregion

        }
    }
}