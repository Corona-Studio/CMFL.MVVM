using System;
using System.Net;

namespace CMFL.MVVM
{
    public class WebClient : System.Net.WebClient
    {
        public WebClient()
        {
            Timeout = 10 * 1000;
        }

        public int Timeout { get; set; }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            var lWebRequest = base.GetWebRequest(uri);
            lWebRequest.Timeout = Timeout;
            if (lWebRequest is HttpWebRequest webRequest) webRequest.ReadWriteTimeout = Timeout;
            return lWebRequest;
        }
    }
}