using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Alfred.Utils.Synology
{
    public class DownloadManager
    {
        const string baseUrl = "http://media.dynalias.org:5000/";
        const string account = "Admin";
        const string passwd = "Ghiltoniel1";
        public string queryCgi = "webapi/query.cgi";
        public string authCgi = "webapi/auth.cgi";
        public string taskCgi = "webapi/DownloadStation/task.cgi";
        private HttpWebRequest request;
        private string sid;
        private bool isAuthenticated;

        private HttpWebRequest GetRequest(string method)
        {
            if (request == null)
                request = new HttpWebRequest(method);
            return request;
        }

        private string Authenticate()
        {
            return GetApiRequest(new Dictionary<string, string> 
            {
                { "api", "SYNO.API.Auth" },
                { "version","2" },
                { "method", "login" },
                { "account", account },
                { "passwd", passwd },
                { "session", "DownloadStation" },
                { "format", "sid" }
            }, "GET", authCgi);
        }

        private void GetApiInfos()
        {
            var response = GetApiRequest(new Dictionary<string, string> 
            {
                { "api", "SYNO.API.Info" },
                { "version","1" },
                { "method", "query" },
                { "query", "SYNO.API.Auth,SYNO.DownloadStation.Task" }
            }, "GET", queryCgi);

            dynamic task = JObject.Parse(response);

            if (task.success == true)
            {
                authCgi = task.data.SYNO.API.Auth;
                taskCgi = task.data.SYNO.DownloadStation.Task;
            }
        }

        public string GetApi(Dictionary<string, string> parameters, string method, string api)
        {
            if (!isAuthenticated)
            {
                //GetApiInfos();
                var response = Authenticate();
                dynamic task = JObject.Parse(response);

                if (task.success == true)
                {
                    sid = task.data.sid;
                    isAuthenticated = true;
                }
                else
                    return null;
            }

            parameters.Add("_sid", sid);
            return GetApiRequest(parameters, method, api);
        }

        private string GetApiRequest(Dictionary<string, string> parameters, string method, string api)
        {
            var request = GetRequest(method);
            var query = "?";

            foreach (var item in parameters)
            {
                query += string.Format("{0}={1}&", item.Key, item.Value);
            }
            query = query.Remove(query.Length - 1);

            return request.Get(string.Format("{0}{1}{2}", baseUrl, api, query));
        }
    }
}