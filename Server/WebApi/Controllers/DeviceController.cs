using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Alfred.Model.Core.Light;
using Alfred.Plugins;
using Alfred.Plugins.Manager;
using Alfred.Utils;
using Alfred.Utils.Managers;
using Alfred.Server.Attributes;
using Alfred.Model.Db.Repositories;
using System.Linq;
using Alfred.Utils.Lights.Hue;
using Alfred.Utils.Lights.Telldus;
using System;

namespace Alfred.Server.WebApi.Controllers
{
    [TokenAuthorize]
    public class DeviceController : ApiController
    {
        private readonly LightManager _lightManager = CommonManagers.LightManager;
        
        [HttpGet]
        [Route("device/interfaces")]
        public virtual IEnumerable<LightInterfaceModel> Interfaces()
        {
            var configurations = new ConfigurationRepository().GetAll();
            var ret = new List<LightInterfaceModel>();
            ret.Add(new LightInterfaceModel()
            {
                Id = "device-hue",
                Name = _lightManager.DeviceInterfaces.Single(t => t is HueInterface).Name,
                Enabled = configurations.Single(c => c.Name == "Device_HueEnabled").Value == "1",
                Configurations = configurations.Where(c => c.Name.StartsWith("Device_Hue"))
            });

            ret.Add(new LightInterfaceModel()
            {
                Id = "device-telldus",
                Name = _lightManager.DeviceInterfaces.Single(t => t is TelldusInterface).Name,
                Enabled = configurations.Single(c => c.Name == "Device_TelldusEnabled").Value == "1",
                Configurations = configurations.Where(c => c.Name.StartsWith("Device_Telldus"))
            });

            return ret;
        }

        [HttpGet]
        [Route("device/hue/bridges")]
        public virtual async Task<IEnumerable<string>> GetHueBridgesIp()
        {
            return await HueInterface.HueHelper.GetBridgesIp();
        }

        [HttpGet]
        [Route("device/hue/bridges/{ip}/register")]
        public virtual async Task<IHttpActionResult> RegisterHueBridge(string ip)
        {
            try {
                var apiKey = await HueInterface.HueHelper.RegisterBridgeIp(
                    ip,
                    string.Concat("device-", Guid.NewGuid()).Substring(0, 19),
                    string.Concat("app-", Guid.NewGuid()).Substring(0, 19)
                );
                var repo = new ConfigurationRepository();
                repo.Save(new Model.Core.ConfigurationModel()
                {
                    Name = "Device_HueBridgeIp",
                    Value = ip
                });
                repo.Save(new Model.Core.ConfigurationModel()
                {
                    Name = "Device_HueBridgeUser",
                    Value = apiKey
                });

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("device/saveInterfaces")]
        public virtual IHttpActionResult SaveInterfaces(IEnumerable<LightInterfaceModel> model)
        {
            var repo = new ConfigurationRepository();
            
            foreach(var m in model)
            {
                foreach(var lc in m.Configurations)
                {
                    repo.Save(lc);
                }
            }

            _lightManager.SetInterfaces();

            return Ok();
        }

        [HttpGet]
        [Route("device/list")]
        public async virtual Task<IEnumerable<LightModel>> List()
        {
            _lightManager.SetDevices();
            return _lightManager.Devices;
        }
        [HttpGet]
        [Route("device/get/{id}")]
        public virtual LightModel Get(string id)
        {
            return _lightManager.GetDevice(id);
        }

        [HttpGet]
        [Route("device/state/{id}/{bri:int}/{hue:int?}/{sat:int?}")]
        public virtual string ChangeState(string id, bool? on = null, byte? bri = null, int? hue = null, int? sat = null)
        {
            _lightManager.Light(id, on, bri, hue, sat);
            return "OK";
        }

        [HttpGet]
        [Route("device/all/{on:bool}")]
        public virtual string ToggleAll(bool on)
        {
            _lightManager.LightAll(on);
            return "OK";
        }
    }
}
