using System.Collections.Generic;
using Alfred.Model.Core;
using Alfred.Utils.Managers;
using Alfred.Utils.Plugins;
using Newtonsoft.Json;
using Alfred.Utils;
using System.Collections.Specialized;

namespace Alfred.Plugins
{
    public class Sensor : BasePlugin
    {
        public override void Initialize()
        {
            // Preload sensors
            CommonManagers.SensorManager.SetDevices().Wait();
            CommonManagers.SensorManager.Devices.CollectionChanged += Sensors_CollectionChanged;
        }

        private void Sensors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            BroadcastSensors();
        }

        public void BroadcastSensors()
        {
            WebSocketServer.Broadcast(new AlfredTask
            {
                Arguments = new Dictionary<string, string>
                {
                    { "sensors", JsonConvert.SerializeObject(CommonManagers.SensorManager.Devices) }
                }
            });
        }

        public void BroadcastSensorHistory()
        {
            var id = arguments["id"];
            var sensor = CommonManagers.SensorManager.GetDevice(id);
            var isensor = CommonManagers.SensorManager.GetDeviceInterface(sensor.Manufacturer);
            client.Send(new AlfredTask
            {
                Command = "Sensor_History",
                Arguments = new Dictionary<string, string>
                {
                    { "id", sensor.Id },
                    { "type", sensor.Type.ToString() },
                    { "history", JsonConvert.SerializeObject(isensor.GetDeviceHistory(sensor.Id)) }
                }
            });
        }
    }
}
