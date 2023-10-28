namespace Bro.Toolbox.Logic.BehaviourTree
{
    public class DelegateValue : EndPoint
    {
        public delegate Result ResultDelegate();

        private readonly ResultDelegate _result;

        public DelegateValue(ResultDelegate result)
        {
            _result = result;
        }

        public override Result Process()
        {
            Result result;
            if (_result == null)
            {
                result = Result.Fail;
            }
            else
            {
                result = _result();
            }

            return result;
        }
    }
}