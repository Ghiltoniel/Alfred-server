using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Alfred.Model.Core.Sensor;
using Alfred.Model.Core.Sensor.NinjaBlocks;
using Newtonsoft.Json;
using RestSharp;

namespace Alfred.Utils.Devices.NinjaBlocks
{
    public class NinjaBlocksInterface : ISensorInterface
    {
        const string API_URL = "https://api.ninja.is/rest/v0";
        const string user_access_token = "d4adf9672b2f9b0cabfba09f92bfe600a5ce4ad2";
        static RestClient client;

        static NinjaBlocksInterface()
        {
            client = new RestClient();
        }

        private string GetUrl(string method, string deviceGuid = null, string parameters = null)
        {
            return string.Format("{0}{1}{2}{3}user_access_token={4}",
                API_URL,
                string.IsNullOrEmpty(method) ? null : "/" + method,
                string.IsNullOrEmpty(deviceGuid) ? null : "/" + deviceGuid,
                string.IsNullOrEmpty(parameters) ? "?" : "/" + parameters + "&",
                user_access_token);
        }

        public async Task<List<SensorModel>> GetDevices()
        {
            var result = new List<SensorModel>();
            var httpClient = new HttpClient();
            var response = await httpClient.GetStringAsync(GetUrl("devices"));
            dynamic dynObj = JsonConvert.DeserializeObject(response);
            if (dynObj.result.Value == 1)
            {
                try
                {
                    foreach (var device in dynObj.data)
                    {
                        if (device.Value.is_sensor == 1)
                        {
                            var newDevice = new NinjaBlocksSensor
                            {
                                Id = device.Name,
                                Manufacturer = SensorManufacturer.NinjaBlocks,
                                Name = device.Value.shortName,
                                IsActuator = device.Value.is_actuator == 1,
                                Type = device.Value.device_type == "temperature" ? SensorType.Temperature :
                                        device.Value.device_type == "humidity" ? SensorType.Humidity : SensorType.Energy,
                                Value = device.Value.last_data.DA != null ? (device.Value.last_data.DA.Value).ToString() : null
                            };
                            if(result.SingleOrDefault(d=>d.Id == newDevice.Id) == null)
                                result.Add(newDevice);
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
            return result;
        }

        public IDictionary<DateTime, double> GetDeviceHistory(string deviceGuid)
        {
            var result = new Dictionary<DateTime, double>();
            var httpClient = new HttpClient();
            var now = DateTime.UtcNow;
            var timestampTo = Math.Round(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);
            var timestampFrom = Math.Round(now.AddDays(-1).Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);
            var response = httpClient.GetStringAsync(GetUrl(
                "device", 
                deviceGuid, 
                string.Format("data?from={0}&to={1}&interval={2}", timestampFrom, timestampTo, "20min"))).Result;
            dynamic dynObj = JsonConvert.DeserializeObject(response);
            if (dynObj.result == 1)
            {
                try
                {
                    var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    foreach (var data in dynObj.data)
                    {
                        var test = data.t.Value;
                        var date = dtDateTime.AddMilliseconds(test).ToLocalTime();
                        result.Add(date, data.v.Value);
                    }
                }
                catch (Exception)
                {

                }
            }
            return result;
        }

        public void AddCallback(string deviceGuid, string callbackUrl)
        {
            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[] 
                {
                    new KeyValuePair<string, string>("url", callbackUrl)
                });
                var result = client.PostAsync(GetUrl("device", deviceGuid, "callback"), content).Result;
                dynamic dynObj = JsonConvert.DeserializeObject(result.Content.ReadAsStringAsync().Result);
                if (dynObj.result == 1)
                {
                    try
                    {
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }
    }
}
