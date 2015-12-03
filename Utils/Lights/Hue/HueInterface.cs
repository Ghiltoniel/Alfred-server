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
        private static ILocalHueClient Client;
        private static List<LightModel> _devices;

        static HueInterface()
        {
            Client = new LocalHueClient(LightConfigurations.HueBridgeIp);
            Client.Initialize(LightConfigurations.HueBridgeUser);
        }

        public async Task<List<LightModel>> GetDevices()
        {            
            try
            {
                var lights = await Client.GetLightsAsync();
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
            var test = Client.SendCommandAsync(new LightCommand
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
                await Client.SendCommandAsync(new LightCommand
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
            await Client.SendCommandAsync(new LightCommand
            {
                On = on
            }, new[] { key });
        }

        public async void ToggleAll(bool on)
        {
            await Client.SendCommandAsync(new LightCommand
            {
                On = on
            });
        }
    }
}