#if UNITY_IOS || UNITY_ANDROID || UNITY_STANDALONE || UNITY_XBOXONE || UNITY_PS4 || UNITY_WEBGL || UNITY_WII
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using Bro.Client;
using UnityEngine;
using UnityEngine.Networking;

namespace Bro.Network
{
    public class UnityWebClient : IWebClient
    {
        private readonly Timing.YieldWaitForEndOfFrame _waitForEndOfFrame = new Timing.YieldWaitForEndOfFrame();
        private readonly IClientContext _clientContext;
        private readonly long _timeout;
        
        public UnityWebClient(IClientContext clientContext, long timeout)
        {
            _clientContext = clientContext;
            _timeout = timeout;
        }

        public void Dispose()
        {
            
        }

        public void SendPostRequest(string url, NameValueCollection postData, Action<string, Exception> resultHandler)
        {
            _clientContext.Scheduler.StartCoroutine(RequestPost(url, postData, resultHandler));
        }

        public void SendGetRequest(string url, Action<string, Exception> resultHandler)
        {
            _clientContext.Scheduler.StartCoroutine(RequestGet(url, resultHandler));
        }

        private IEnumerator RequestPost(string url, NameValueCollection postData, Action<string, Exception> resultHandler)
        {
            var post = new WWWForm();
            var items = postData.AllKeys.SelectMany(postData.GetValues, (k, v) => new {key = k, value = v});
            foreach (var item in items)
            {
                post.AddField(item.key, item.value);
            }

            using (var request = UnityWebRequest.Post(url, post))
            {
                request.timeout = (int) (_timeout / 1000);
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    yield return _waitForEndOfFrame;
                }

                if (!request.isNetworkError && !request.isHttpError)
                {
                    resultHandler.Invoke(request.downloadHandler.text, null);
                }
                else
                {
                    Bro.Log.Error("web client :: url = " + url + ", error " + request.error);
                    resultHandler.Invoke(null, new Exception(request.error));
                }
            }
        }

        private IEnumerator RequestGet(string url, Action<string, Exception> resultHandler)
        {
            using (var request = UnityWebRequest.Get(url))
            {
                request.timeout = (int) (_timeout / 1000);
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    yield return _waitForEndOfFrame;
                }

                if (!request.isNetworkError && !request.isHttpError)
                {
                    resultHandler.Invoke(request.downloadHandler.text, null);
                }
                else
                {
                    Bro.Log.Error("web client :: url = " + url + ", error " + request.error);
                    resultHandler.Invoke(null, new Exception(request.error));
                }
            }
        }
    }
}
#endif