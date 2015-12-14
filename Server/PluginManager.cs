using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Alfred.Plugins.Managers;
using Alfred.Model.Db;
using Alfred.Utils.Managers;
using Alfred.Utils.Plugins;
using log4net;
using Alfred.Model.Db.Repositories;
using Alfred.Model.Core;

namespace Alfred.Plugins.Manager
{
    public class PluginManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PluginManager));
        private List<Type> _pluginTypes;
        private IList<Assembly> _externalAssemblies;
        private ConfigurationRepository _repo;

        public Dictionary<string, BasePlugin> LoadedPlugins;

        public PluginManager()
        {
            _repo = new ConfigurationRepository();
        }

        public void LoadPlugins()
        {
            _externalAssemblies = new List<Assembly>();

            LoadedPlugins = new Dictionary<string, BasePlugin>();
            _pluginTypes = AppDomain.CurrentDomain.GetAssemblies()
                       .Where(a => a.GetName().Name == "Alfred.Plugins")
                       .Union(AppDomain.CurrentDomain.GetAssemblies()
                       .Where(a => a.GetName().Name == "Alfred.Server"))
                       .SelectMany(assembly => assembly.GetTypes())
                       .Where(type => type.IsSubclassOf(typeof(BasePlugin)))
                       .OrderBy(t => t.Name)
                       .ToList();

            var directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "lib");
            if(!directory.Exists)
            {
                directory.Create();
            }

            foreach (var file in directory.GetFiles())
            {
                if (file.Extension != ".dll")
                    continue;

                var externalAssembly = Assembly.LoadFrom(file.FullName);
                _externalAssemblies.Add(externalAssembly);
                _pluginTypes.AddRange(externalAssembly.GetTypes()
                    .Where(type => type.IsSubclassOf(typeof(BasePlugin))));
            }

            var configsDb = _repo.GetAll();

            foreach (var type in _pluginTypes)
            {
                var pluginEnabled = configsDb.SingleOrDefault(c => c.Name == type.Name + "_Enabled");

                if (pluginEnabled == null)
                {
                    var config = new ConfigurationModel
                    {
                        Name = type.Name + "_Enabled",
                        Value = "1"
                    };
                    _repo.Save(config);
                    pluginEnabled = config;
                }

                if (pluginEnabled.Value == "0")
                    continue;

                log.Info(string.Format("Loading plugin {0} ...", type.Name));
                var plugin = LoadPlugin(type);
                plugin.Initialize();
                log.Info(string.Format("Plugin {0} loaded !", type.Name));

                log.Info(string.Format("Loading plugin {0} configuration ...", type.Name));
                var pluginConfigs = configsDb
                    .Where(c => c.Name.StartsWith(type.Name + "_"))
                    .ToDictionary(k => k.Name.Replace(type.Name + "_", ""), k => k.Value);
                plugin.LoadConfig(pluginConfigs);
                log.Info(string.Format("Plugin {0} configuration loaded !", type.Name));
            }
        }

        public BasePlugin GetPlugin(string typeName)
        {
            if (LoadedPlugins.Keys.Contains(typeName))
            {
                return LoadedPlugins[typeName];
            }
            return null;
        }

        public BasePlugin LoadPlugin(Type type)
        {
            BasePlugin plugin;
            if (type.Name == "MediaManager")
                plugin = CommonPlugins.media;
            else if (type.Name == "Alfred")
                plugin = CommonPlugins.alfred;
            else
            {
                // Create a new object of plugin type
                plugin = Activator.CreateInstance(type) as BasePlugin;
            }

            plugin.WebSocketServer = Init.WebSocketServer;
            plugin.name = type.Name;
            LoadedPlugins.Add(plugin.name, plugin);
            return plugin;
        }
    }
}
