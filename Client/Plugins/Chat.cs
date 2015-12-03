
using System.Collections.Generic;
using Alfred.Model.Core;

namespace Alfred.Client.Plugins
{
    public class Chat
    {
        public void SendMessage(string message)
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Chat_Send",
                Arguments = new Dictionary<string, string>
                {
                    { "text", message }
                }
            });
        }
    }
}
