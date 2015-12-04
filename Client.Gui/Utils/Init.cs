using System;
using Alfred.Client.Core;
using Alfred.Client.Gui.Properties;
using Alfred.Model.Core;
using Newtonsoft.Json;
using WebSocket4Net;

namespace Alfred.Client.Gui
{
    public class Init
    {
        public static AlfredClient alfredClient;
        public static MainWindow mainWindow;

        static Init()
        {
            alfredClient = new AlfredClient(
                Settings.Default.ServerName,
                Settings.Default.ServerHost,
                Settings.Default.ServerPort,
                Settings.Default.ServerLogin,
                Settings.Default.ServerPassword,
                OnReceive);
        }

        public static void OnReceive(AlfredTask task)
        {
            if (task.Command == "StartStopSpeech" && task.Type == TaskType.Server)
                mainWindow.ToggleSpeech();
            else if (task.Command == "StartStopKinect" && task.Type == TaskType.Server)
                mainWindow.ToggleKinect();
            else if (task.Command == "Done" && task.Type == TaskType.Server)
                mainWindow.ToggleSpeech(true);
        }
    }
}
