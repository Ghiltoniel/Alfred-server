using Alfred.Client.Http;
using System;
using System.Net.Http;

namespace Alfred.Client
{
    public class AlfredPluginsHttp
    {
        public Music Music;
        public Sensor Sensor;
        public Light Light;
        public Scenario Scenario;
        public Http.Alfred Alfred;

        public AlfredPluginsHttp(string url = "http://api-nam.kicks-ass.org/")
        {
            var httpClient = new AlfredHttpClient()
            {
                BaseAddress = new Uri(url)
            };
                
            Music = new Music(httpClient);
            Sensor = new Sensor(httpClient);
            Light = new Light(httpClient);
            Scenario = new Scenario(httpClient);
            Alfred = new Http.Alfred(httpClient);
        }
    }
}
