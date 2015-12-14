using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Alfred.Model.Core.Light;
using Newtonsoft.Json;

namespace Alfred.Client.Core.Http
{
    public class Light : BaseHttpPlugin
    {
        public Light(AlfredHttpClient client):base(client)
        {

        }

        public async Task<IEnumerable<LightModel>> GetAll()
        {            
            var result = await _httpClient.GetAsync("device/list");
            if (result.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<IEnumerable<LightModel>>(await result.Content.ReadAsStringAsync());
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
