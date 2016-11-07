using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;

namespace EM.Util
{
    public class HttpPostWrapper 
    {

        public Uri URL { get; private set; }
        public HttpWebRequest Request { get; private set; }
        public HttpWebResponse Response { get; private set; }
        public String ResponseString { get; private set; } 

        public HttpPostWrapper(Uri url)
        {
            this.URL = url;
            this.Request = (HttpWebRequest)WebRequest.Create(this.URL);
            this.Request.Method = "POST";
            this.Request.AllowAutoRedirect = true;
            this.Request.ContentType = "application/x-www-form-urlencoded";
        }

        /// <summary>
        /// post data using ASCII encoding
        /// </summary>
        public void PostData(string postData)
        {
            PostData(postData, Encoding.ASCII);
        }

        /// <summary>
        /// if leaving ContentType default (application/x-www-form-urlencoded) then postData must be in a format like: HttpUtility.UrlEncode("a=b&c=d")
        /// </summary>
        public void PostData(string postData, Encoding enc)
        {
            var data = enc.GetBytes(postData);
            this.Request.ContentLength = data.Length;
            var reqStream = this.Request.GetRequestStream();
            reqStream.Write(data, 0,data.Length);
            reqStream.Close();

            this.Response = (HttpWebResponse)this.Request.GetResponse();
            var respStream = this.Response.GetResponseStream();
            var reader = new StreamReader(respStream);
            this.ResponseString = reader.ReadToEnd();
            respStream.Close();
            this.Response.Close();
        }

        /// <summary>
        /// verify if Response.StatusCode is HttpStatusCode.OK aka HTTP status 200 
        /// </summary>
        public bool IsResultOK
        {
            get
            {
                return Response.StatusCode == HttpStatusCode.OK;
            }
        }


    }
}
