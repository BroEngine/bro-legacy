using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace Bro.Network.Web
{
    public class WebClient : System.Net.WebClient, IWebClient
    {
        private Action<byte[], Exception> _resultCallbackBytes;
        private Action<string, Exception> _resultCallbackString;

        private readonly bool _keepAlive;
        private readonly long _timeout;
        private string _url;
        
        private readonly Stopwatch _timer = new Stopwatch();

        public WebClient(bool keepAlive, long timeout)
        {
            _keepAlive = keepAlive;
            _timeout = timeout;
            _timer.Start();
            DownloadDataCompleted += (sender, e) => { OnWebClientResult(e); };
            UploadValuesCompleted += (sender, e) => { OnWebClientResult(e); };
        }

        static WebClient()
        {
            ServicePointManager.CheckCertificateRevocationList = false;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.ServerCertificateValidationCallback += RemoteCertificateValidationCallback;
        }

        private static bool RemoteCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true; // whatever lol
        }

        /* признавайтесь кто впердолил в каком то текстовом редакторе бомы */
        /* для utf8 первые 3 байта BOM ( byte order mask ) EF BB BF */
        private static byte[] RemoveBomUtf8(byte[] responseBytes)
        {
            if (responseBytes.Length >= 3 && responseBytes[0] == 239 && responseBytes[1] == 187 && responseBytes[2] == 191)
            {
                var dst = new byte[responseBytes.Length - 3];
                Array.Copy(responseBytes, 3, dst, 0, dst.Length);
                responseBytes = dst;
            }
            return responseBytes;
        }
        
        private void OnWebClientResult(DownloadDataCompletedEventArgs args)
        {
            try
            {
                var error = args.Error;

                if (error != null)
                {
                    Bro.Log.Info("Web client exception, url = " + _url + ", error = " + error.Message + ", timeout = " + _timeout + ", elapsed = " + _timer.ElapsedMilliseconds);
                    InvokeStringResult(null, error);
                    InvokeBytesResult(null, error);
                    return;
                }

                var responseBytes = RemoveBomUtf8 ( args.Result );
                var responseString = System.Text.Encoding.UTF8.GetString(responseBytes);

                InvokeStringResult(responseString, null);
                InvokeBytesResult(responseBytes, null);
            }
            catch (Exception ex)
            {
                Log.Info(ex.GetType().ToString());
                Log.Error(ex);
                InvokeStringResult(null, ex);
                InvokeBytesResult(null, ex);
            }
        }

        private void OnWebClientResult(UploadValuesCompletedEventArgs args)
        {
            try
            {
                var error = args.Error;

                if (error != null)
                {
                    Bro.Log.Info("Web client exception, url = " + _url + ", error = " + error.Message);
                    InvokeStringResult(null, error);
                    InvokeBytesResult(null, error);
                    return;
                }

                var responseBytes = RemoveBomUtf8 ( args.Result );
                var responceString = System.Text.Encoding.UTF8.GetString(responseBytes);

                InvokeStringResult(responceString, null);
                InvokeBytesResult(responseBytes, null);
            }
            catch (Exception ex)
            {
                Log.Info(ex.GetType().ToString());
                Log.Error(ex);
                InvokeStringResult(null, ex);
                InvokeBytesResult(null, ex);
            }
        }

        private void InvokeStringResult(string result, Exception ex)
        {
            if (_resultCallbackString != null)
            {
                _resultCallbackString.Invoke(result, ex);
                _resultCallbackString = null;
            }
        }

        private void InvokeBytesResult(byte[] result, Exception ex)
        {
            if (_resultCallbackBytes != null)
            {
                _resultCallbackBytes.Invoke(result, ex);
                _resultCallbackBytes = null;
            }
        }

        private void Reset()
        {
            if (_resultCallbackBytes != null)
            {
                Bro.Log.Error("WebClient contains not invoked callback(bytes), will be reseted");
                _resultCallbackBytes = null;
            }

            if (_resultCallbackString != null)
            {
                Bro.Log.Error("WebClient contains not invoked callback(string), will be reset");
                _resultCallbackString = null;
            }
        }

        protected override System.Net.WebRequest GetWebRequest(Uri address)
        {
            var request = (HttpWebRequest)base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = (int) _timeout;
                request.KeepAlive = _keepAlive; 
            }

            return request;
        }

        public void SendPostRequest(string url, NameValueCollection postData, Action<string, Exception> resultHandler)
        {
            _url = url;
            Reset();
            _resultCallbackString = resultHandler;

            UploadValuesAsync(new Uri(url), "POST", postData);
        }

        public void SendGetRequest(string url, Action<string, Exception> resultHandler)
        {
            _url = url;
            Reset();
            _resultCallbackString = resultHandler;
            
            DownloadDataAsync(new Uri(url));   
        }
    }
}