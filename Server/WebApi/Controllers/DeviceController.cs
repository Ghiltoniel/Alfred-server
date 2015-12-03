using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Alfred.Model.Core.Light;
using Alfred.Plugins;
using Alfred.Plugins.Manager;
using Alfred.Utils;
using Alfred.Utils.Managers;

namespace Alfred.Server.WebApi.Controllers
{
    public class DeviceController : ApiController
    {
        private readonly LightManager _lightManager = CommonManagers.LightManager;

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
