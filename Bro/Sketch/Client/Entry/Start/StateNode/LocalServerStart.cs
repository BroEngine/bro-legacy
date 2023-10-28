using Bro.Toolbox.Logic.BehaviourTree;

namespace Bro.Sketch.Client
{
    public class LocalServerStart : EndPoint
    {
        public override Result Process()
        {
#if UNITY_EDITOR
            if (DebugSettings.Instance.StartLocalServer)
            {
                // new LocalServer.World().Start();
                // new LocalServer.Battle().Start();
            }
#endif
            return Result.Success;
        }
    }
}