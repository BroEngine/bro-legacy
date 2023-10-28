using System;
using System.Collections.Specialized;

namespace Bro.Network
{
    public interface IWebClient : IDisposable
    {
        void SendPostRequest(string url, NameValueCollection postData, Action<string, Exception> resultHandler);
        void SendGetRequest(string url, Action<string, Exception> resultHandler);
    }
}