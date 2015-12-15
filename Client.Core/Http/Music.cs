using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Alfred.Model.Core.Music;
using Newtonsoft.Json;

namespace Alfred.Client.Core.Http
{
    public class Music : BaseHttpPlugin
    {
        public Music(AlfredHttpClient client):base(client)
        {

        }

        public async Task<IEnumerable<Song>> GetTournedisqueNews()
        {            
            var result = await _httpClient.GetAsync("tournedisque");
            if (result.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<IEnumerable<Song>>(await result.Content.ReadAsStringAsync());
            return null;
        }

        public async Task<IEnumerable<Song>> GetTournedisqueGenre(string genre)
        {
            var result = await _httpClient.GetAsync("tournedisque/genres/" + genre);
            if (result.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<IEnumerable<Song>>(await result.Content.ReadAsStringAsync());
            return null;
        }

        public async Task<IDictionary<string, string>> GetTournedisqueGenres()
        {
            try
            {
                var result = await _httpClient.GetAsync("tournedisque/genres");
                if (result.IsSuccessStatusCode)
                    return JsonConvert.DeserializeObject<IDictionary<string, string>>(await result.Content.ReadAsStringAsync());
                return null;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Radio>> GetRadios()
        {
            var result = await _httpClient.GetAsync("radios");
            if (result.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<IEnumerable<Radio>>(await result.Content.ReadAsStringAsync());
            return null;
        }

        public async Task<Radio> GetRadio(string basename)
        {
            var result = await _httpClient.GetAsync("radios/"+basename);
            if (result.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<Radio>(await result.Content.ReadAsStringAsync());
            return null;
        }

        public async Task<IEnumerable<SubRadio>> GetSubRadios(string radioName)
        {
            var result = await _httpClient.GetAsync("radios/subradios/" + radioName);
            if (result.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<IEnumerable<SubRadio>>(await result.Content.ReadAsStringAsync());
            return null;
        }

        public async Task<IEnumerable<GroovesharkSong>> SearchGrooveshark(string search)
        {
            var url = string.Format("http://tinysong.com/s/{0}?format=json&limit=32&key=91c9333f318105b007f6dc49ac07fc03", search);
            var res = new HttpClient();
            var result = await res.GetAsync(url); 
            if (result.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<IEnumerable<GroovesharkSong>>(await result.Content.ReadAsStringAsync());
            return null;
        }
    }
}
