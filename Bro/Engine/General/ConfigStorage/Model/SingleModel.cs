using Bro.Json;

namespace Bro
{
    public class SingleModel<T>
    {
        [JsonProperty("data")] public T Data;
    }
}