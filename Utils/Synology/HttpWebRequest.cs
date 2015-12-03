using System.IO;
using System.Net;

namespace Alfred.Utils.Synology
{
    public class HttpWebRequest
    {
        private readonly string method;

        public HttpWebRequest(string method)
        {
            this.method = method;
        }

        public string Get(string sURL)
        {
            WebRequest wrGETURL;
            wrGETURL = WebRequest.Create(sURL);
            wrGETURL.Method = method;

            Stream objStream;
            objStream = wrGETURL.GetResponse().GetResponseStream();

            var objReader = new StreamReader(objStream);
            return objReader.ReadToEnd();
        }
    }
}