
using System.Collections.Generic;
using Alfred.Model.Core;

namespace Alfred.Client.Core.Plugins
{
    public class Player
    {
        public void Register(string name)
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Player_Register",
                Arguments = new Dictionary<string, string>
                {
                    { "name", name }
                }
            });
        }

        public void Unregister()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Player_Unregister"
            });
        }

        public void ReadyToPlay()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Player_ReadyToPlay"
            });
        }
    }
}
