namespace Bro.Client
{
    public partial class Timing
    {
        public partial class Ease
        {
            public class Parallel : Base
            {
                private Update _update;
                
                private readonly Base[] _easeRange;
                
                public Parallel(params Base[] easeRange)
                {
                    _easeRange = easeRange;
                }

                protected override void Update(float dt)
                {
                    if (!IsRunning)
                    {
                        return;
                    }
                    
                    for (var i = 0; i < _easeRange.Length; i++)
                    {
                        var ease = _easeRange[i];
                        if (ease.IsRunning)
                        {
                            return;
                        }
                    }
                    
                    Stop();
                }

                public override void Start()
                {
                    for (var i = 0; i < _easeRange.Length; i++)
                    {
                        var ease = _easeRange[i];
                        ease.Setup(_scheduler);
                        ease.Start();
                    }
                    
                    _update = new Update(Update, _scheduler);
                    _update.Start();
                    
                    base.Start();
                }

                public override void Stop()
                {
                    base.Stop();
                    
                    for (var i = 0; i < _easeRange.Length; i++)
                    {
                        var ease = _easeRange[i];
                        ease.Stop();
                    }
                    
                    _update.Stop();
                }
            }
        }
    }
}