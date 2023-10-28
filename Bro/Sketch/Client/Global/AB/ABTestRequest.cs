using System;
using Bro.Json;
using Bro.Sketch;

namespace Bro.Core.Client
{
    [Serializable]
    public class ABTestRequest
    {
        [JsonProperty("version")] public int Version;
        [JsonProperty("device_id")] public string DeviceId;
        [JsonProperty("google_id")] public string GoogleId;
        [JsonProperty("apple_id")] public string AppleId;
        [JsonProperty("facebook_id")] public string FacebookId;
        
        public ABTestRequest( int version, string deviceId, string googleId, string appleId, string facebookId )
        {
            Version = version;
            DeviceId = deviceId;
            GoogleId = googleId;
            AppleId = appleId;
            FacebookId = facebookId;
        }
        
        public static ABTestRequest Create(int version, string appleId,string googleId, string facebookId) 
        {
            var deviceId = Bro.Client.DeviceId.Value;
            return new ABTestRequest( version, deviceId, googleId, appleId, facebookId );
        }
    }
    
    [Serializable]
    public class ABTestResponse
    {
        [JsonProperty("test_id")] public int TestId;
        [JsonProperty("category_id")] public int CategoryId;
        [JsonProperty("category_index")] public int CategoryIndex;
        [JsonProperty("category_name")] public string CategoryName;
    }
}