namespace Bro.Toolbox.Logic.BehaviourTree
{
    public class UtilityProvider : IUtility
    {
        public delegate float UtilityValueDelegate();

        private readonly UtilityValueDelegate UtilityValueProvider;

        public float UtilityValue
        {
            get { return UtilityValueProvider(); }
        }

        public string UtilityType => GetType().Name;

        public void Reset()
        {
        }

        public UtilityProvider(UtilityValueDelegate utilityValueProvider)
        {
            UtilityValueProvider = utilityValueProvider;
        }
    }
}

