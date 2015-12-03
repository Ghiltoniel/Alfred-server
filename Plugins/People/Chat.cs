using System.Collections.Generic;
using Alfred.Model.Core;
using Alfred.Utils.Managers;
using Alfred.Utils.Plugins;

namespace Alfred.Plugins
{
    public class Chat : BasePlugin
    {
        public override void Initialize()
        {
 
        }

        public void Send()
        {
            var token = arguments["token"];
            var login = UserManager.GetUserLogin(token);
            WebSocketServer.Broadcast(new AlfredTask
            {
                Command = "Chat_Sent",
                Arguments = new Dictionary<string, string>
                {
                        { "message", arguments["text"] },
                        { "user", login }
                    }
            });
        }
    }
}
