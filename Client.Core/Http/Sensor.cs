using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Alfred.Model.Core.Sensor;

namespace Alfred.Client.Core.Http
{
    public class Sensor : BaseHttpPlugin
    {
        public Sensor(AlfredHttpClient client):base(client)
        {

        }

        public async Task<IEnumerable<SensorModel>> GetAll()
        {            
            var result = await _httpClient.GetAsync("sensor/list");
            if (result.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<IEnumerable<SensorModel>>(await result.Content.ReadAsStringAsync());
            return null;
        }

        public async Task<IDictionary<DateTime, double>> GetHistory(string sensorId)
        {
            var result = await _httpClient.GetAsync("sensor/history/" + sensorId);
            if (result.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<IDictionary<DateTime, double>>(await result.Content.ReadAsStringAsync());
            return null;
        }
    }
}
