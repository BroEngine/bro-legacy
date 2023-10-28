using System.Collections.Generic;
using Bro.Json;

namespace Bro.Server.Context
{
    public class OnlineCounter
    {
        public class Model
        {
            [JsonProperty("total")] public int Total;
            [JsonProperty("context")] public Dictionary<string, int> Context;
        }

        public Model Count(List<IServerContext> contexts)
        {
            var model = new Model {Context = new Dictionary<string, int>()};
            for (int i = 0; i < contexts.Count; ++i)
            {
                var contextType = contexts[i].GetType().Name;
                var online = contexts[i].PeersAmount;
                if (model.Context.ContainsKey(contextType))
                {
                    model.Context[contextType] += online;
                }
                else
                {
                    model.Context.Add(contextType, online);
                }

                model.Total += online;
            }

            return model;
        }
    }
}