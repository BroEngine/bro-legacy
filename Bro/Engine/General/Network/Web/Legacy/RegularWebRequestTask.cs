using System;
using System.Collections.Specialized;

namespace Bro.Network.Web
{
    public class RegularWebRequestTask : SubscribableTask<RegularWebRequestTask>
    {
        public enum RequestType
        {
            Post,
            Get,
        }

        private readonly IWebClient _client;
        private readonly string _url;
        private readonly NameValueCollection _postData;
        private readonly RequestType _requestType;

        public string Response { get; private set; }

        public RegularWebRequestTask(IWebContext webContext, RequestType requestType, string url, NameValueCollection postData = null)
        {
            if (webContext == null)
            {
                Bro.Log.Info("temp :: init config in some context");
                _client = new WebClient(true, 5000);
            }
            else
            {
                _client = webContext.GetWebClient();
            }

           
            _url = url;
            _postData = postData;
            _requestType = requestType;
        }

        protected override void Activate(ITaskContext taskContext)
        {
            base.Activate(taskContext);

            switch (_requestType)
            {
                case RequestType.Post:
                    _client.SendPostRequest(_url, _postData, RequestHandler);
                    break;
                case RequestType.Get:
                    _client.SendGetRequest(_url, RequestHandler);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RequestHandler(string response, Exception exception)
        {
            Response = response;
            
            _client.Dispose();
            
            if (exception != null)
            {
                Bro.Log.Info("Request " + _url + " failed with exception = " + exception);
                Fail(exception);
            }
            else
            {
                Complete();
            }
        }
    }
}