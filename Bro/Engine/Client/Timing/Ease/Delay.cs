namespace Bro.Client
{
    public partial class Timing
    {
        public partial class Ease
        {
            public class Delay : Base
            {
                private Update _update;

                public Delay(float duration)
                {
                    _duration = duration;
                }
                
                protected override void Update(float dt)
                {
                    if (_elapsedTime < _duration)
                    {
                        _elapsedTime += dt;
                    }
                    else
                    {
                        Stop();
                    }
                }
                
                public override void Start()
                {
                    _update = new Update(Update, _scheduler);
                    _update.Start();
                    
                    base.Start();
                }

                public override void Stop()
                {
                    base.Stop();
                    
                    _update.Stop();
                }
            }
        }
    }
}