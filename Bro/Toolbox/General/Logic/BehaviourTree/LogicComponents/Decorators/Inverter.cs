namespace Bro.Toolbox.Logic.BehaviourTree
{
    public class Inverter : Decorator
    {
        public override Result Process()
        {
            Result result;
            switch (Child.Process())
            {
                case Result.Success:
                    result = Result.Fail;
                    break;
                case Result.Fail:
                    result = Result.Success;
                    break;
                case Result.Running:
                    result = Result.Running;
                    break;
                case Result.Restart:
                    result = Result.Restart;
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }

            return result;
        }
        
    }
}