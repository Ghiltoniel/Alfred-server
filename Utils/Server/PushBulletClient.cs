using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Alfred.Utils.Server
{
    public class PushBulletClient
    {
        private readonly string _apiKey;
        public PushBulletClient(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<bool> PushNoteToAll(string title, string body)
        {
            var client = new HttpClient {BaseAddress = new Uri("https://api.pushbullet.com/v2/")};
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
            string postData =
                "{\"type\": \"note\", \"title\": \""+title+"\", \"body\": \"" + body + "\"}";
            
            var response = await client.PostAsync("pushes", new StringContent(postData, Encoding.UTF8, "application/json"));
            return response.IsSuccessStatusCode;
        }
    }
}
