using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Http;
using Alfred.Model.Core;
using Alfred.Model.Core.Light;
using Alfred.Model.Core.Scenario;
using Alfred.Plugins;
using Alfred.Server.Plugins;
using Alfred.Model.Db;
using Alfred.Utils.Radios;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using Alfred.Plugins.Manager;
using Alfred.Model.Db.Repositories;
using Alfred.Server.Attributes;

namespace Alfred.Server.WebApi.Controllers
{
    [TokenAuthorize]
    public class ScenarioController : ApiController
    {
        public ScenarioRepository _repo;
        public ScenarioController()
        {
            _repo = new ScenarioRepository();
        }

        [HttpGet]
        [Route("scenario/list")]
        public IEnumerable<ScenarioModel> GetAll()
        {
            return _repo.GetAll();
        }

        [HttpPost]
        [Route("scenario/save")]
        public HttpResponseMessage Save(ScenarioModel scenario)
        {
            if(string.IsNullOrEmpty(scenario.Name))
            {
                ModelState.AddModelError("Name", "Le champ Name est requis");
            }

            if(!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (!string.IsNullOrEmpty(scenario.Radio))
            {
                var infosRadio = scenario.Radio.Split('-');
                if (infosRadio.Length > 0)
                {
                    var radio = ARadio.GetARadio(infosRadio[0]);
                    var subradios = radio.GetAllSubRadios();
                    if (subradios != null)
                        foreach (var subradio in subradios)
                            if (subradio.Name == infosRadio[1])
                                scenario.RadioUrl = subradio.Url;
                    if (scenario.RadioUrl == null)
                        scenario.RadioUrl = radio.BaseUrl;
                }
            }
            try
            {
                _repo.Save(scenario);
            }
            catch (DbEntityValidationException e)
            {
                var test = e.EntityValidationErrors;
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, e);
            }
            new Interface().ReloadSpeech();

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("scenario/{scenarioId}")]
        public ScenarioModel Get(int scenarioId)
        {
            var scenario = _repo.Get(scenarioId);
            return scenario;
        }

        [HttpGet]
        [Route("scenario/run/{id:int}")]
        public string Run(int id)
        {
            var existingScenario = _repo.Get(id);
            if (existingScenario != null)
            {
                var task = new AlfredTask
                {
                    Command = "Scenario_LaunchScenario",
                    Type = TaskType.Server,
                    SpeakBeforeExecute = true,
                    SpeakAfterExecute = true
                };
                task.Arguments = new Dictionary<string, string>();
                task.Arguments.Add("mode", existingScenario.Name);

                Launcher.EnqueueTask(task);
                return "OK";
            }
            return "Error";
        }

        [HttpGet]
        [Route("scenario/run/{name}")]
        public string RunByName(string name)
        {
            var existingScenario = _repo.GetByName(name);
            if (existingScenario != null)
            {
                var task = new AlfredTask
                {
                    Command = "Scenario_LaunchScenario",
                    Type = TaskType.Server,
                    SpeakBeforeExecute = true,
                    SpeakAfterExecute = true
                };
                task.Arguments = new Dictionary<string, string>();
                task.Arguments.Add("mode", existingScenario.Name);

                Launcher.EnqueueTask(task);
                return "OK";
            }
            return "Error";
        }
    }
}
