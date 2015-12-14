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
        private readonly ConnectionDetails _connection;
        private List<TellStickLiveDevice> _devices;

        public TelldusInterface(string ck, string csk, string t, string ts)
        {
            _connection = new ConnectionDetails(
                ck,
                csk,
                t,
                ts);

            _devices = new List<TellStickLiveDevice>();
        }

        public string Name
        {
            get
            {
                return "Telldus";
            }
        }

        public async Task<List<LightModel>> GetDevices()
        {
            try
            {
                await Task.Run(() =>
                {
                    try
                    {
                        _devices = TellstickLiveController.GetDevices(_connection);
                    }
                    catch (Exception)
                    {
                        _devices = new List<TellStickLiveDevice>();
                    }
                });

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
                        new Task(() => TellstickLiveController.Dim(id, (int)bri, _connection)) :
                        new Task(() => TellstickLiveController.Action(id, on.HasValue && on.Value ? "turnOn" : "turnOff", _connection));

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
                Task task = new Task(() => TellstickLiveController.Action(id, on ? "turnOn" : "turnOff", _connection));
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
                var task = new Task(() => TellstickLiveController.TurnAll(on, _connection));
                task.Start();
                await task;
            }
            catch (Exception)
            {

            }
        }
    }
}