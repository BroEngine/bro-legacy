using System;
using System.Collections.Specialized;
using Bro.Encryption;
using Bro.Json;
using Bro.Json.Linq;

// use example
//var request = new Bro.Network.Web.NetworkRequest<TestResponse>("http://example.org", new TestRequest());
//request.Load((response) =>
//{ 
//});

namespace Bro.Network.Web
{
    public static class WebRequest
    {
        public static bool Debug = false;
        public static bool EncryptionDebug = false;
    }

    public class WebRequest<T> where T : new()
    {
        private const int Protocol = 1;

        private const string KeyPrefix = "Rw+%]Y^}/zrcJJ;,_6m<,&9C?vg)#{[n";
        private const string KeySuffix = "_.=gan!&4C6^ppX',5+j6:cVH/};S%5M";

        private readonly IWebClient _client;
        private readonly string _url;
        private readonly object _requestData;
        private readonly PostData _postData;
        private readonly bool _silence;

        private class PostData
        {
            [JsonProperty("time")] public long Time;
            [JsonProperty("protocol")] public int Protocol;
            [JsonProperty("data")] public string Data;
        }

        public class Response
        {
            [JsonProperty("result")] public bool Result;
            [JsonProperty("protocol")] public int Protocol;
            [JsonProperty("time")] public long Time;
            [JsonProperty("error")] public string Error;
            [JsonProperty("data")] public T Data;

            public static Response CreateError( string error )
            {
                return new Response()
                {
                    Result = false,
                    Error = error
                };
            }
        }

        public WebRequest(IWebContext webContext, string url, object requestData = null, bool silence = false)
        {
            _url = url;
            _silence = silence;
            _requestData = requestData;
           
            if (webContext == null)
            {
                Bro.Log.Info("temp :: init config in some context");
                _client = new WebClient(true, 5000);
            }
            else
            {
                _client = webContext.GetWebClient();
            }
            
            _postData = new PostData()
            {
                Time = TimeInfo.GlobalTimestamp,
                Protocol = Protocol
            };
        }
        private static string GenerateKey(long time)
        {
            return Tools.MD5(KeyPrefix + time + KeySuffix);
        }

        public void Load(Action<Response> responseCallback)
        {
            var post = new NameValueCollection() {};
            
            if (_requestData != null)
            {
                if (!_silence || WebRequest.Debug)
                {
                    Bro.Log.Info("web :: requesting (post) url = " + _url, 2);
                }

                var key = GenerateKey(_postData.Time);
                var encryptor = new AES(key);
                
                if (WebRequest.EncryptionDebug)
                {
                    Bro.Log.Info("web :: generated encryption key = " + key + " for time = " + _postData.Time);
                }

                var serializedData = JsonConvert.SerializeObject(_requestData, Formatting.None, JsonConverters.Settings);
                var encryptedData = encryptor.Encrypt(serializedData);

                encryptedData = Random.Instance.String(4) + encryptedData;

                _postData.Data = encryptedData;
                var postData = JsonConvert.SerializeObject(_postData, Formatting.None, JsonConverters.Settings);

                if (WebRequest.Debug)
                {
                    Bro.Log.Info("#WEB, serialized data = " + serializedData, 2);
                    Bro.Log.Info("#WEB, encrypted data = " + encryptedData, 2);
                    Bro.Log.Info("#WEB, POST data = " + postData, 2);
                }

                post = new NameValueCollection() {{"request", postData}};
            }
            
            _client.SendPostRequest(_url, post, (r, e) => { OnResponse(r, e, responseCallback); });
        }

        private void OnResponse(string responseString, Exception ex, Action<Response> responseCallback)
        {        
            #if UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID || UNITY_EDITOR
            Bro.Log.Info("web :: on response url = " + _url + ", result = " + (!string.IsNullOrEmpty(responseString)) + ", ex = " + ( ex != null ), 2);
            #endif
            
            _client.Dispose();

            var response = new Response()
            {
                Result = false,
                Error = "Client side error, failed to parse response = " + responseString,
            };

            if (WebRequest.Debug)
            {
                Bro.Log.Info("#WEB, request for url = " + _url + " response = " + responseString, 2);
            }

            if (string.IsNullOrEmpty(responseString) || ex != null)
            {
                if (responseCallback != null)
                {
                    responseCallback.Invoke(response);
                }

                return;
            }

            JObject jsonObject = null;
            try
            {
                jsonObject = JObject.Parse(responseString);
            }
            catch
            {
                Bro.Log.Info("Cannot parse response = " + responseString + ", " + _url);
                if (responseCallback != null)
                {
                    responseCallback.Invoke(response);
                }

                return;
            }

            var jsonResult = jsonObject.SelectToken("result");
            var jsonTime = jsonObject.SelectToken("time");
            var jsonProtocol = jsonObject.SelectToken("protocol");
            var jsonError = jsonObject.SelectToken("error");
            var jsonData = jsonObject.SelectToken("data");

            if (jsonTime != null)
            {
                response.Time = jsonTime.Value<long>();
            }

            if (jsonProtocol != null)
            {
                response.Protocol = jsonProtocol.Value<int>();
            }

            if (jsonError != null)
            {
                response.Error = jsonError.Value<string>();
            }

            if (jsonResult == null || jsonResult.Value<int>() == 0)
            {
                Bro.Log.Info("#WEB, request for url = " + _url + " return result = 0, error = " + response.Error);
                if (responseCallback != null)
                {
                    responseCallback.Invoke(response);
                }

                return;
            }

            var jsonDataString = string.Empty;

            if (jsonData != null)
            {
                jsonDataString = jsonData.Value<string>();
            }

            if (string.IsNullOrEmpty(jsonDataString))
            {
                Bro.Log.Info("#WEB, request for url = " + _url + " return result = 1, but faild to get data property");
                if (responseCallback != null)
                {
                    responseCallback.Invoke(response);
                }

                return;
            }

            if (jsonDataString.Length <= 4)
            {
                Bro.Log.Info("#WEB, request for url = " + _url + " return result = 1, but faild on first phase of decrypting");
                if (responseCallback != null)
                {
                    responseCallback.Invoke(response);
                }

                return;
            }

            jsonDataString = jsonDataString.Substring(4);

            var key = GenerateKey(response.Time);
            
            var encryptor = new AES(key);

            if (WebRequest.EncryptionDebug)
            {
                Bro.Log.Info("#WEB, generated decryption key = " + key + " for time = " + _postData.Time);
            }

            jsonDataString = encryptor.Decrypt(jsonDataString);

            if (WebRequest.Debug)
            {
                Bro.Log.Info("#WEB, decrypted data for url = " + _url + " is " + jsonDataString);
            }
            
            response.Data = JsonConvert.DeserializeObject<T>(jsonDataString, JsonConverters.Settings);
            if (response.Data != null)
            {
                response.Result = true;
            }

            if (response.Result)
            {
                if (!_silence || WebRequest.Debug)
                {
                    Bro.Log.Info("web :: response for url = " + _url + " result = " + response.Result, 1);
                }
            }
            else
            {
                Bro.Log.Info("web :: response for url = " + _url + " result = " + response.Result, 1);
            }

            if (responseCallback != null)
            {
                responseCallback.Invoke(response);
            }
        }

        public void Dispose()
        {
            if (_client != null)
            {
                _client.Dispose();
            }
        }
    }
}