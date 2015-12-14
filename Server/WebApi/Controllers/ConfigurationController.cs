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

        [HttpPost]
        [Route("save")]
        public IHttpActionResult Save(ConfigurationModel model)
        {
            _repo.Save(model);

            return Ok();
        }

        [HttpPost]
        [Route("saveBatch")]
        public IHttpActionResult SaveBatch(IEnumerable<ConfigurationModel> model)
        {
            foreach (var c in model) {
                _repo.Save(c);
            };

            Launcher.LoadPlugins();
            return Ok();
        }

    }
}
