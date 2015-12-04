using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Alfred.Model.Core.Scenario;
using Newtonsoft.Json;

namespace Alfred.Client.Core.Http
{
    public class Scenario : BaseHttpPlugin
    {
        public Scenario(AlfredHttpClient client):base(client)
        {

        }

        public async Task<IEnumerable<ScenarioModel>> GetAll()
        {
            var result = await _httpClient.GetAsync("scenario/list");
            if (result.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<IEnumerable<ScenarioModel>>(await result.Content.ReadAsStringAsync());
            return null;
        }

        public async Task<ScenarioModel> Get(int scenarioId)
        {
            var result = await _httpClient.GetAsync("scenario/" + scenarioId);
            if (result.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<ScenarioModel>(await result.Content.ReadAsStringAsync());
            return null;
        }

        public async Task<bool> Save(ScenarioModel scenario)
        {
            var queryString = new StringContent(JsonConvert.SerializeObject(scenario));
            queryString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.PostAsync("scenario/save", queryString);
            return result.IsSuccessStatusCode;
        }
    }
}
