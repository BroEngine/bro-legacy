using Bro.Json;

namespace Bro.Toolbox.General.Logic.BehaviourTree.BehaviourTreeEditor.TreeModel
{
    public class TreeModel
    {
        public class TestUtility
        {
            [JsonProperty("x")] public float X;
            [JsonProperty("y")] public float Y;
            [JsonProperty("max_radius")] public float MaxRadius;
            [JsonProperty("min_radius")] public float MinRadius;
            [JsonProperty("direction_angle")] public float DirectionAngle = 90;
            [JsonProperty("spread_angle")] public float SpreadAngle = 45;
            [JsonProperty("visualize")] public bool Visualize;
        }
    }
}