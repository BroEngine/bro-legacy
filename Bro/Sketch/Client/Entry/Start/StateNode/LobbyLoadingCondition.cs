using Bro.Toolbox.Logic.BehaviourTree;

namespace Bro.Sketch.Client
{
    public class LobbyLoadingCondition : Condition
    {
        public LobbyLoadingCondition()
        {
            ConditionHandler = IsTutorialPassed;
        }

        private bool IsTutorialPassed()
        {
            return true;
        }
    }
}