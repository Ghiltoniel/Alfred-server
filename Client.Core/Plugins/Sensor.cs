using System.Collections.Generic;
using Alfred.Model.Core;

namespace Alfred.Client.Core.Plugins
{
    public class Sensor
    {
        public void BroadcastSensors()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Sensor_BroadcastSensors"
            });
        }

        public void BroadcastSensorHistory(string id)
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Sensor_BroadcastSensorHistory",
                Arguments = new Dictionary<string, string>
                {
                    { "id", id }
                }
            });
        }
    }
}
