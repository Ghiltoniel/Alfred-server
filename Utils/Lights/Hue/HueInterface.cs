using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Alfred.Model.Core.Light;
using Alfred.Utils.Lights;
using Q42.HueApi;
using Q42.HueApi.Interfaces;

namespace Alfred.Utils.Lights.Hue
{
    public class HueInterface : ILightInterface
    {
        private ILocalHueClient _client;
        private List<LightModel> _devices;

        public HueInterface(string ip, string appKey)
        {
            _client = new LocalHueClient(ip);
            _client.Initialize(appKey);
        }

        public string Name
        {
            get
            {
                return "Philips Hue";
            }
        }

        public async Task<List<LightModel>> GetDevices()
        {            
            try
            {
                var lights = await _client.GetLightsAsync();
                _devices = lights.Select(l => new LightModel(
                    l.Id,
                    l.Name,
                    l.State.On,
                    l.State.Brightness,
                    DeviceType.Hue,
                    true,
                    true,
                    l.State.Saturation,
                    l.State.Hue)).ToList();

                return _devices;
            }
            catch(Exception)
            {
                return new List<LightModel>();
            }
        }

        public void Light(string key, bool? on = null, byte? bri = null, int? hue = null, int? sat = null)
        {
            var test = _client.SendCommandAsync(new LightCommand
            {
                Brightness = bri,
                Hue = hue,
                Saturation = sat,
                On = on
            }, new[] { key }).Result;
        }

        public async void LightAll(bool? on = null, byte? bri = null, int? hue = null, int? sat = null)
        {
            try
            {
                await _client.SendCommandAsync(new LightCommand
                {
                    Brightness = bri,
                    Hue = hue,
                    Saturation = sat,
                    On = on
                });
            }
            catch (Exception e)
            {

            }
        }

        public async void Toggle(string key, bool on)
        {
            await _client.SendCommandAsync(new LightCommand
            {
                On = on
            }, new[] { key });
        }

        public async void ToggleAll(bool on)
        {
            await _client.SendCommandAsync(new LightCommand
            {
                On = on
            });
        }

        public static class HueHelper
        {
            public static async Task<IEnumerable<string>> GetBridgesIp()
            {
                IBridgeLocator locator = new HttpBridgeLocator();
                return await locator.LocateBridgesAsync(TimeSpan.FromSeconds(5));
            }

            public static async Task<string> RegisterBridgeIp(string ip, string appName, string deviceName)
            {
                ILocalHueClient client = new LocalHueClient(ip);
                return await client.RegisterAsync(appName, deviceName);
            }
        }
    }
}