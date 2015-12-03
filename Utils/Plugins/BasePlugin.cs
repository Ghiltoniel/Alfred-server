using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Alfred.Model.Core;
using Alfred.Model.Core.Plugins;
using Alfred.Model.Core.WebSocket;
using Alfred.Model.Db;
using Alfred.Utils.Managers;
using Alfred.Utils.Utils;
using log4net;
using Alfred.Model.Db.Repositories;

namespace Alfred.Utils.Plugins
{
    public class BasePlugin
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BasePlugin));

        public Client client;
        public string name;
        public IAlfredWebSocketServer webSocketServer;
        public Dictionary<string, string> arguments;
        public Dictionary<string, string> configs;
        public PluginResult result = new PluginResult();

        public IAlfredWebSocketServer WebSocketServer
        {
            get
            {
                return webSocketServer;
            }
            set
            {
                webSocketServer = value;
            }
        }

        public virtual void Initialize()
        {
            var configurations = new ConfigurationRepository().GetAll();
            configs = configurations
                .Where(c => c.Name.StartsWith(name + "_"))
                .ToDictionary(c => c.Name, c => c.Value);
        }

        public virtual bool BeforeExecute(AlfredTask task, MethodInfo method)
        {
            var authAttribute = Attribute.GetCustomAttribute(GetType(), typeof(WebSocketAuthorize));
            var methodAuthAttribute = Attribute.GetCustomAttribute(method, typeof(WebSocketAuthorize));
            var _isSecured = authAttribute == null && methodAuthAttribute == null;
            if (_isSecured)
            {
                log.Info("Method is secured. Checking credentials...");
                var userToken =
                    task.Arguments != null
                    && task.Arguments.ContainsKey("token") ?
                    UserManager.ValidateToken(task.Arguments["token"]) :
                    null;

                if (string.IsNullOrEmpty(userToken))
                {
                    log.Info("User is not logged in.");
                    client.Send(new AlfredTask
                    {
                        Command = "Unauthorized"
                    });
                    return false;
                }

                log.Info("User is authorized.");
                var data = Convert.FromBase64String(userToken);
                var when = DateTime.FromBinary(BitConverter.ToInt64(data, 0));
                if (string.IsNullOrEmpty(client.Name))
                {
                    task.Client.Name = UserManager.GetUserLogin(userToken);
                }
                if (when < DateTime.UtcNow.AddHours(-24))
                {
                    UserManager.SetUserToken(client);
                }
                return true;
            }
            return true;
        }

        public void LoadConfig(Dictionary<string, string> configsDb)
        {
            configs = configsDb;
        }
    }
}
