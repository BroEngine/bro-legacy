using System.Collections.Generic;

using Bro.Json;
using Bro.Sketch;
using UnityEngine;

namespace Bro.Toolbox.Navigation
{
    [System.Serializable]
    public class Path
    {
        [JsonProperty(PropertyName = "p")] public List<Vector2> Points = new List<Vector2>();

        [JsonIgnoreAttribute]
        public Vector2 Start => Points[0];

        [JsonIgnoreAttribute]
        public Vector2 Finish => Points[Points.Count - 1];

        [JsonIgnoreAttribute]
        public float Lenght
        {
            get
            {
                float result = 0f;
                for (int i = 1, max = Points.Count; i < max; ++i)
                {
                    result += (Points[i] - Points[i - 1]).magnitude;
                }

                return result;
            }
        }

        public Path()
        {
        }

        public Path(Vector2 start, Vector2 finish)
        {
            Points.Add(start);
            Points.Add(finish);
        }
    }
}