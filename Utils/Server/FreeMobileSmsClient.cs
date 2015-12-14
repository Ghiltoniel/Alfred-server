using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Alfred.Utils.Properties;

namespace Alfred.Utils.Server
{
    public class FreeMobileSmsClient
    {
        private readonly string _user;
        private readonly string _pass;

        public FreeMobileSmsClient(string user, string pass)
        {
            _user = user;
            _pass = pass;
        }

        public async Task SendSms(string body)
        {
            var test = new HttpClient { BaseAddress = new Uri("https://smsapi.free-mobile.fr/") };
            await test.GetStringAsync("sendmsg?user=" + _user + "&pass=" + _pass + "&msg=" + body);
        }
    }
}
