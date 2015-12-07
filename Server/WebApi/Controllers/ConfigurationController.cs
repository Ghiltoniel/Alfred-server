using Alfred.Model.Core;
using Alfred.Model.Db;
using Alfred.Model.Db.Repositories;
using Alfred.Server.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Alfred.Server.WebApi.Controllers
{
    [TokenAuthorize]
    [RoutePrefix("configuration")]
    public class ConfigurationController : ApiController
    {
        ConfigurationRepository _repo = new ConfigurationRepository();

        [HttpGet]
        [Route("")]
        public IEnumerable<ConfigurationModel> GetAll()
        {
            return _repo.GetAll();
        }

        [HttpGet]
        [Route("save")]
        public IHttpActionResult Save(string name, string value)
        {
            _repo.Save(new ConfigurationModel()
            {
                Name = name,
                Value = value
            });

            return Ok();
        }
    }
}
