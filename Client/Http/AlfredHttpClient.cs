using Alfred.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Alfred.Client.Http
{
    public class AlfredHttpClient : HttpClient
    {
        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(AlfredClient.Token))
            {
                DefaultRequestHeaders.Add("token", AlfredClient.Token);
            }
            return base.SendAsync(request, cancellationToken);
        }
    }
}
