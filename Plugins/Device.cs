using System.Collections.Generic;
using System.Linq;
using Alfred.Model.Core;
using Alfred.Utils;
using Alfred.Utils.Lights;
using Alfred.Utils.Managers;
using Alfred.Utils.Plugins;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace Alfred.Plugins
{
    public class Device : BasePlugin
    {
        public override void Initialize()
        {
            base.Initialize();

            // Preload lights
            CommonManagers.LightManager.SetDevices();
            CommonManagers.LightManager.CollectionChanged += Devices_CollectionChanged;
        }

        private void Devices_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            BroadcastLights();
        }

        public void LightCommand()
        {
            var on = arguments.ContainsKey("on") ? bool.Parse(arguments["on"]) as bool? : null;
            var bri = arguments.ContainsKey("bri") ? byte.Parse(arguments["bri"]) as byte? : null;
            var hue = arguments.ContainsKey("hue") ? int.Parse(arguments["hue"]) as int? : null;
            var sat = arguments.ContainsKey("sat") ? int.Parse(arguments["sat"]) as int? : null;
            CommonManagers.LightManager.Light(arguments["id"], on, bri, hue, sat);
        }

        public void BroadcastLights()
        {
            WebSocketServer.Broadcast(new AlfredTask
            {
                Arguments = new Dictionary<string, string>
                {
                    { "lights", JsonConvert.SerializeObject(CommonManagers.LightManager.Devices) }
                }
            });
        }

        public void AllumeTout()
        {
            CommonManagers.LightManager.LightAll(true);
        }

        public void EteinsTout()
        {
            CommonManagers.LightManager.LightAll(false);
        }

        public void Allume()
        {
            var light = CommonManagers.LightManager.Devices.Single(d => d.Name == arguments["piece"]);
            CommonManagers.LightManager.Light(light.Key, true);
        }

        public void Eteins()
        {
            var light = CommonManagers.LightManager.Devices.Single(d => d.Name == arguments["piece"]);
            CommonManagers.LightManager.Light(light.Key, false);
        }

        public void TurnUp()
        {
            var light = CommonManagers.LightManager.Devices.Single(d => d.Name == arguments["piece"]);
            CommonManagers.LightManager.Light(light.Key, true, 255);
        }

        public void TurnDown()
        {
            var light = CommonManagers.LightManager.Devices.Single(d => d.Name == arguments["piece"]);
            CommonManagers.LightManager.Light(light.Key, true, 8);
        }
    }
}
