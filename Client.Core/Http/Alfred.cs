using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using Alfred.Model.Core;

namespace Alfred.Client.Core.Http
{
    public class Alfred : BaseHttpPlugin
    {
        public Alfred(AlfredHttpClient client):base(client)
        {

        }

        public async Task<bool> LaunchCommand(IEnumerable<AlfredTask> tasks)
        {
            var queryString = new StringContent(JsonConvert.SerializeObject(tasks));
            queryString.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var result = await _httpClient.PostAsync("alfred/launchcommands", queryString);
            return result.IsSuccessStatusCode;
        }
    }
}
