namespace Bro.Client
{
    public partial class Timing
    {
        public partial class Ease
        {
            public class Sequence : Base
            {
                private Update _update;
                
                private readonly Base[] _easeRange;

                private int _currentIndex;
                
                public Sequence(params Base[] easeRange)
                {
                    _easeRange = easeRange;
                }

                protected override void Update(float dt)
                {
                    if (!IsRunning)
                    {
                        return;
                    }
                    
                    var ease = _easeRange[_currentIndex];
                    if (ease.IsRunning)
                    {
                        return;
                    }
                    
                    _currentIndex++;
                    if (_currentIndex >= _easeRange.Length)
                    {
                        Stop();
                        return;
                    }
                    
                    StartNext();
                }

                public override void Start()
                {
                    StartNext();
                    
                    _update = new Update(Update, _scheduler);
                    _update.Start();
                    
                    base.Start();
                }

                public override void Stop()
                {
                    base.Stop();

                    if (_currentIndex < _easeRange.Length)
                    {
                        var ease = _easeRange[_currentIndex];
                        ease.Stop();
                    }
                    
                    _update.Stop();
                }

                private void StartNext()
                {
                    var ease = _easeRange[_currentIndex];
                    ease.Setup(_scheduler);
                    ease.Start();
                }
            }
        }
    }
    
    
}