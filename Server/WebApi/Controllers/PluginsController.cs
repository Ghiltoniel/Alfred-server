using Alfred.Model.Core.Plugins;
using Alfred.Plugins.Manager;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web.Http;

namespace Alfred.Server.WebApi.Controllers
{
    public class PluginsController : ApiController
    {
        [HttpGet]
        [Route("plugins/list")]
        public List<PluginModel> List(string[] commands)
        {
            var model = new List<PluginModel>();
            var plugins = Launcher.PluginManager.LoadedPlugins;
            foreach(var plugin in plugins)
            {
                var type = plugin.Value.GetType();
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                model.Add(new PluginModel(){ Name = plugin.Key, Methods = methods.Select(m => m.Name).ToList()});
            }
            return model;
        }
    }
}
