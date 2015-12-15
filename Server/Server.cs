using Alfred.Plugins.Managers;
using Alfred.Server.Properties;
using Alfred.Utils.Managers;
using log4net;
using log4net.Config;
using Microsoft.Owin.Hosting;
using System;

namespace Alfred.Server
{
    public class Server
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Server));
        
        static Server()
        {
            XmlConfigurator.Configure();
            new Configurator();
        }

        public static void Start()
        {
            try
            {
                Init.WebSocketServer = new WebSocketServer();

                CommonPlugins.alfred.Speak("Veuillez patienter, je charge l'ensemble de vos modules");
                log.Info("Loading plugins ...");
                Launcher.LoadPlugins();
                log.Info("Loaded plugins !");

                var webApiAddress = string.Format("http://{0}:{1}",
                    Settings.Default.WebApiServerHostname,
                    Settings.Default.WebApiServerPort);

                WebApp.Start<Startup>(webApiAddress);
                log.Info(string.Format("WebApi Server started at : {0}", webApiAddress));

                var webSocketAddress = string.Format("ws://{0}:{1}",
                    Settings.Default.WebSocketServerHostname,
                    Settings.Default.WebSocketServerPort);

                Init.WebSocketServer.Start(webSocketAddress);
                log.Info(string.Format("WebSocket Server started at : {0}", webSocketAddress));
            }
            catch (Exception e)
            {
                log.ErrorFormat("Message : {0}. Inner Exception : {1}",
                        e.Message,
                        e.InnerException != null ? e.InnerException.ToString() : string.Empty);
                throw;
            }
        }
    }
}
