using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alfred.Client.Http
{
    public class BaseHttpPlugin
    {
        protected AlfredHttpClient _httpClient;

        public BaseHttpPlugin(AlfredHttpClient httpClient)
        {
            _httpClient = httpClient;
        }
    }
}
