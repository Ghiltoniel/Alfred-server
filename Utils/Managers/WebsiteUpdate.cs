using System.Collections.Generic;
using Alfred.Model.Core;
using Newtonsoft.Json;

namespace Alfred.Utils.Managers
{
    public static class WebsiteUpdate
    {
        public static void UpdateServerStatus()
        {
            var task = new AlfredTask
            {
                Type = TaskType.WebsiteInfo,
                Command = "UpdateServerStatus",
                Arguments = new Dictionary<string, string>
                {
                        { "status", JsonConvert.SerializeObject(Init.serverStatus) }
                    }
            };

            Init.WebSocketServer.Broadcast(task);
        }
    }
}
