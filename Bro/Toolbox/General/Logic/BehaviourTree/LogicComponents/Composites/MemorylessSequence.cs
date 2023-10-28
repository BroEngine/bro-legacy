﻿using Bro.Toolbox.Logic.BehaviourTree;

namespace Bro.Toolbox.Nodes.BehaviourTree
{
    public class MemorylessSequence : Composite
    {
        public override void Reset()
        {
        }

        public override Result Process()
        {
            var result = Result.Fail;

            if (ChildrenCount > 0)
            {
                for (int i = 0, max = ChildrenCount; i < max; ++i)
                {
                    result = GetChildAtIndex(i).Process();
                    if (result == Result.Fail || result == Result.Restart || result == Result.Running)
                    {
                        break;
                    }
                }
            }

            return result;
        }
    }
}