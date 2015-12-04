using System.Collections.Generic;
using Alfred.Model.Core;

namespace Alfred.Client.Core.Plugins
{
    public class Lights
    {
        public void LightCommand(string id, bool? on = null, byte? bri = null, int? hue = null, int? sat = null)
        {
            var arguments = new Dictionary<string, string>
            {
                { "id", id }
            };

            if(on.HasValue)
                arguments.Add("on", on.Value.ToString());
            if(bri.HasValue)
                arguments.Add("bri", bri.Value.ToString());
            if(hue.HasValue)
                arguments.Add("hue", hue.Value.ToString());
            if(sat.HasValue)
                arguments.Add("sat", sat.Value.ToString());

            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Device_LightCommand",
                Arguments= arguments
            });
        }

        public void BroadcastLights()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Device_BroadcastLights"
            });
        }

        public void AllumeTout()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Device_AllumeTout"
            });
        }

        public void EteinsTout()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Device_EteinsTout"
            });
        }

        public void Allume(string piece)
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Device_Allume",
                Arguments = new Dictionary<string, string>
                {
                    { "piece", piece }
                }
            });
        }

        public void Eteins(string piece)
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Device_Eteins",
                Arguments = new Dictionary<string, string>
                {
                    { "piece", piece }
                }
            });
        }

        public void TurnUp(string piece)
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Device_TurnUp",
                Arguments = new Dictionary<string, string>
                {
                    { "piece", piece }
                }
            });
        }

        public void TurnDown(string piece)
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Device_TurnDown",
                Arguments = new Dictionary<string, string>
                {
                    { "piece", piece }
                }
            });
        }
    }
}
