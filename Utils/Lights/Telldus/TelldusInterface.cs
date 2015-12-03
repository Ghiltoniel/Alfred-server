using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Alfred.Model.Core.Light;
using Tellstick;

namespace Alfred.Utils.Lights.Telldus
{
    public class TelldusInterface : ILightInterface
    {
        static readonly ConnectionDetails Connection;
        static List<TellStickLiveDevice> _devices;

        static TelldusInterface()
        {
            Connection = TelldusLight.connection;
            _devices = new List<TellStickLiveDevice>();
        }

        public async Task<List<LightModel>> GetDevices()
        {
            try
            {
                var task = new Task(GetTellstickDevicesAsync);
                task.RunSynchronously();
                return _devices.Select(telldus =>
                    new LightModel(
                    telldus.id.ToString(CultureInfo.InvariantCulture),
                    telldus.name, 
                    telldus.state > 0 && telldus.state != 2,
                    (byte)telldus.state,
                    DeviceType.Telldus,
                    telldus.methods == 19)).ToList();
            }
            catch (WebException)
            {
                return new List<LightModel>();
            }
        }

        private void GetTellstickDevicesAsync()
        {
            try
            {
                _devices = TellstickLiveController.GetDevices(Connection);
            }
            catch (Exception)
            {
                _devices = new List<TellStickLiveDevice>();
            }
        }

        public async void Light(string key, bool? on = null, byte? bri = null, int? hue = null, int? sat = null)
        {
            int id;
            var res = int.TryParse(key, out id);
            var deviceModel = CommonManagers.LightManager.GetDevice(key);

            if (!res || deviceModel == null)
                return;

            try
            {                
                var task = bri.HasValue && deviceModel.DimEnabled ?
                        new Task(() => TellstickLiveController.Dim(id, (int)bri, Connection)) :
                        new Task(() => TellstickLiveController.Action(id, on.HasValue && on.Value ? "turnOn" : "turnOff", Connection));

                task.RunSynchronously();
                await task;
            }
            catch (Exception)
            {

            }
        }

        public async void LightAll(bool? on = null, byte? bri = null, int? hue = null, int? sat = null)
        {
            if (!bri.HasValue && on.HasValue)
                ToggleAll(on.Value);
            else
                foreach (var device in CommonManagers.LightManager.DevicesType(DeviceType.Telldus))
                    Light(device.Key, on, bri, hue, sat);
        }

        public async void Toggle(string key, bool on)
        {
            int id;
            var res = int.TryParse(key, out id);
            var deviceModel = CommonManagers.LightManager.GetDevice(key);

            if (!res || deviceModel == null)
                return;

            try
            {
                Task task = new Task(() => TellstickLiveController.Action(id, on ? "turnOn" : "turnOff", Connection));
                task.RunSynchronously();
                await task;
            }
            catch (Exception)
            {

            }
        }

        public async void ToggleAll(bool on)
        {
            try
            {
                var task = new Task(() => TellstickLiveController.TurnAll(on, Connection));
                task.Start();
                await task;
            }
            catch (Exception)
            {

            }
        }
    }
}