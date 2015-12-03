using System;
using System.Diagnostics;
using System.Linq;
using AlfredModel;
using AlfredUtils.Plugins;
using Microsoft.Web.Administration;

namespace AlfredServer.Plugins
{
    public class Interface : BasePlugin
    {
        #region Kinect, Speech, Tablet, TTS
        public void StartStopKinect()
        {
            var task = new AlfredTask
            {
                Command = "StartStopKinect",
                Type = TaskType.Server
            };
            WebSocketServer.Broadcast(task);
        }

        public void StartStopRecognition()
        {
            var task = new AlfredTask
            {
                Command = "StartStopSpeech",
                Type = TaskType.Server
            };
            WebSocketServer.Broadcast(task);
        }

        public void RestartPlayer()
        {
            var players = Process.GetProcessesByName("AlfredPlayer");
            if(players.Any())
            {
                foreach (var player in players)
                    player.Kill();
            }
            Process.Start(@"C:\Users\guillaume\Documents\Visual Studio 2012\Projects\Domotique\Domotique\AlfredPlayer\bin\Debug\AlfredPlayer.exe");
        }

        public void RestartWebsite()
        {
            var manager = new ServerManager();
            var site = manager.Sites.FirstOrDefault(s => s.Name == arguments["WebsiteName"]);
            try
            {
                if (site != null)
                {
                    site.Stop();
                    site.Start();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void ReloadPlugins()
        {
            Launcher.LoadPlugins();
            result.toSpeakString = "Veuillez patienter, je recharge les modules";
        }

        public void ReloadSpeech()
        {

        }

        public void SearchTablet()
        {

        }
        #endregion
    }
}
