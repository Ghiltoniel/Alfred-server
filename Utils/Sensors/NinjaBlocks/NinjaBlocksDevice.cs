using System.Collections.Generic;
using RestSharp;

namespace Alfred.Utils.NinjaBlocks
{
    public class Device
    {
        const string API_URL = "https://api.ninja.is/rest/v0/";
        private RestClient client;

        public Device()
        {
            client = new RestClient();
        }

        public HashSet<DeviceModel> GetList()
        {
            return new HashSet<DeviceModel>();
        }
    }
}
