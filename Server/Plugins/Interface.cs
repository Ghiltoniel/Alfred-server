using System;
using System.Diagnostics;
using System.Linq;
using Alfred.Model.Core;
using Alfred.Utils.Plugins;
using Microsoft.Web.Administration;
using Alfred.Utils;

namespace Alfred.Server.Plugins
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

        public void ReloadPlugins()
        {
            Launcher.LoadPlugins();
            result.toSpeakString = "Veuillez patienter, je recharge les modules";
        }

        public void ReloadDevices()
        {
            CommonManagers.LightManager.SetInterfaces();
            result.toSpeakString = "Veuillez patienter, je recharge les lumières";
        }

        public void ReloadSpeech()
        {
            WebSocketServer.Broadcast(new AlfredTask()
            {
                Command = "ReloadSpeech"
            });
        }
        #endregion
    }
}
