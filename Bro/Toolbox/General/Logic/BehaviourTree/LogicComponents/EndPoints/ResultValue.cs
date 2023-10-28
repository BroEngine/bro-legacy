namespace Bro.Toolbox.Logic.BehaviourTree
{
    public class ResultValue : EndPoint
    {
        private readonly Result _value;

        public ResultValue(Result value)
        {
            _value = value;
        }

        public override Result Process()
        {
            return _value;
        }
    }
}