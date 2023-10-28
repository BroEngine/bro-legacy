using Bro.Json;

namespace Bro.Toolbox
{
    public static class ObjectExtensions
    {
        private static JsonSerializerSettings Settings(bool writeTypes = true)
        {
            var result = new JsonSerializerSettings() {TypeNameHandling = writeTypes ? TypeNameHandling.All : TypeNameHandling.Auto};
            return result;
        }

        public static string ToJsonString(this object o, bool writeTypes = true)
        {
            return JsonConvert.SerializeObject(o, Formatting.Indented, Settings(writeTypes));
        }
    }
}