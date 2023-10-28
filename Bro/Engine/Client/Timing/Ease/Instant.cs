using System;

namespace Bro.Client
{
    public partial class Timing
    {
        public partial class Ease
        {
            public class Instant : Base
            {
                private readonly Action _action;

                public Instant(Action action)
                {
                    _action = action;
                }
                
                protected override void Update(float dt)
                {

                }

                public override void Start()
                {
                    _action.Invoke();
                }
            }
        }
    }
}