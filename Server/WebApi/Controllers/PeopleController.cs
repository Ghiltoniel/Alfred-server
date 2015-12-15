using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Alfred.Server.Properties;
using Alfred.Utils.Server;
using Newtonsoft.Json;
using RestSharp;
using Alfred.Server.Attributes;

namespace Alfred.Server.WebApi.Controllers
{
    [TokenAuthorize]
    public class PeopleController : ApiController
    {
        [HttpGet]
        [Route("people/home/{name}")]
        public async Task Home(string name)
        {
            var existingPeople = ServerData.Peoples.SingleOrDefault(p => p.Name == name);
            if (existingPeople != null)
                existingPeople.IsHome = true;
            else
            {
                ServerData.Peoples.Add(new People()
                {
                    Name = name,
                    IsHome = true
                });
            }

            await new PushBulletClient(Settings.Default.PushBulletApiKey)
                .PushNoteToAll("NAM", name + " est arrivé à la maison !");
        }

        [HttpGet]
        [Route("people/left/{name}")]
        public virtual void Left(string name)
        {
            var existingPeople = ServerData.Peoples.SingleOrDefault(p => p.Name == name);
            if (existingPeople != null)
                existingPeople.IsHome = false;
            else
            {
                ServerData.Peoples.Add(new People()
                {
                    Name = name,
                    IsHome = false
                });
            }
        }
    }
}
