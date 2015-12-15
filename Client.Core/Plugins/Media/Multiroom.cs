
using System.Collections.Generic;
using Alfred.Model.Core;

namespace Alfred.Client.Core.Plugins
{
    public class Multiroom
    {
        public void SynchronizePlayer(string playerName, int channel)
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Multiroom_SynchronizePlayer",
                Arguments = new Dictionary<string, string>
                {
                    { "name", playerName },
                    { "channel", channel.ToString() }
                }
            });
        }

        public void SynchronizePlayers(int channel)
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Multiroom_SynchronizePlayers",
                Arguments = new Dictionary<string, string>
                {
                    { "channel", channel.ToString() }
                }
            });
        }
    }
}
