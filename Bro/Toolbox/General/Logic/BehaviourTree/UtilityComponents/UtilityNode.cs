namespace Bro.Toolbox.Logic.BehaviourTree
{
    public class UtilityNode : Decorator
    {
        private readonly IUtility _utilityProvider;

        [BehaviourContent("type")] public string UtilityProviderType => _utilityProvider.UtilityType;
        [BehaviourContent("ut")] public float UtilityValue => _utilityProvider.UtilityValue;
        public UtilityNode(IUtility utilityProvider)
        {
            _utilityProvider = utilityProvider;
        }

        public override Result Process()
        {
            return Child.Process();
        }

        public override void OnExit()
        {
            _utilityProvider.Reset();
            base.OnExit();
        }
    }
}