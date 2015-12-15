using System.Globalization;
using System.Net;
using System.Text;
using Alfred.Utils.Plugins;
using Newtonsoft.Json.Linq;

namespace Alfred.Plugins
{
	public class Meteo : BasePlugin
	{
		public double Temperature {get;set;}
		public string Phenomene {get;set;}
        
		public void MeteoToday()
        {
            const string url = "http://api.openweathermap.org/data/2.5/weather?q=Paris,fr";

            var client = new WebClient {Encoding = Encoding.UTF8};

		    try
            {
                var infos = client.DownloadString(url);
                dynamic json = JObject.Parse(infos);
                double temp = json.main.temp.Value;

                client.Dispose();

                result.toSpeakString = "Il fait " + (temp - 273.15).ToString(CultureInfo.InvariantCulture).Replace('.', ',') + " degrés";
            }
            catch (WebException)
            {
                result.toSpeakString = "Désolé, mais je ne parviens pas à me connecter au serveur météorologique !";
            }
		}
	}
}
