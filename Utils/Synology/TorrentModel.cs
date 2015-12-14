using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Alfred.Utils.Synology
{
    public class TorrentModel
    {	
        public static bool AddUrl(string url)
        {
            var manager = new DownloadManager();
            var parameters = new Dictionary<string, string> 
            {
                { "api", "SYNO.DownloadStation.Task" },
                { "version","1" },
                { "method", "create" },
                { "uri", url }
            };
            var response = manager.GetApi(parameters, "POST", manager.taskCgi);

            dynamic task = JObject.Parse(response);
            return (task.success == true);
        }
    }
}