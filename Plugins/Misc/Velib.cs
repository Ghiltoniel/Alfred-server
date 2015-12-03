using System.Collections.Generic;
using System.Net.Http;
using Alfred.Utils.Plugins;
using Newtonsoft.Json;

namespace Alfred.Plugins
{
    public class Velib : BasePlugin
    {
        public override void Initialize()
        {

        }

        public void GetNearestAvailabilities()
        {
            var url = string.Format("https://api.jcdecaux.com/vls/v1/stations?contract={0}&apiKey={1}",
                "paris",
                "e1afe62d10328d88185a0bb94c4e02bea16672cc");

            var result = JsonConvert.DeserializeObject<IEnumerable<Station>>(new HttpClient().GetStringAsync(url).Result);
            foreach (var line in result)
            {
                if (line.name.Contains("VERON"))
                {
                    if (line.status == "OPEN")
                    {
                        this.result.toSpeakString = string.Format("Il reste {0} vélos disponibles et {1} emplacements libre", line.available_bikes, line.available_bike_stands);
                    }
                    else
                        this.result.toSpeakString = string.Format("La station est indisponible");
                }
            }
        }

        class Station
        {
            public int number { get; set; }
            public string contract_name { get; set; }
            public string name { get; set; }
            public string address { get; set; }
            public bool banking { get; set; }
            public bool bonus { get; set; }
            public string status { get; set; }
            public byte bike_stands { get; set; }
            public byte available_bike_stands { get; set; }
            public byte available_bikes { get; set; }
            public long last_update { get; set; }
        }
    }
}
