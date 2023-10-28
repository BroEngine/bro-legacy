namespace Bro.Toolbox.Logic.BehaviourTree
{
    public class ResultSetter : Decorator
    {
        private readonly Result _result;

        public ResultSetter(Result returnResult)
        {
            _result = returnResult;
        }

        public override Result Process()
        {
            Child?.Process();
            return _result;
        }
    }
}