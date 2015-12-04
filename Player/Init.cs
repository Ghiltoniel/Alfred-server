using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Alfred.Client.Core;
using Alfred.Model.Core;
using AlfredPlayer.Properties;
using Newtonsoft.Json;
using WebSocket4Net;

namespace AlfredPlayer
{
    public class Init
    {
        public static Dictionary<string, string> config;
        public static MediaManager media;
        public static WebBrowser browser;
        public static Player form;
        public static Label connectionLabel;
        public static Button connectionButton;
        public static AlfredClient alfredClient;
        delegate void ConnectionStatusDelegate();
        public static int ping;

        static Init()
        {
            alfredClient = new AlfredClient(
                Settings.Default.ServerName,
                Settings.Default.ServerHost,
                Settings.Default.ServerPort,
                Settings.Default.ServerLogin,
                Settings.Default.ServerPassword,
                OnReceive,
                OnDisconnect,
                OnConnect);
        }

        static void ChangeConnectionStatus()
        {
            connectionLabel.Text = "Connected !";
            connectionButton.Enabled = false;
        }

        static void ChangeConnectionStatus2()
        {
            connectionLabel.Text = "Disconnected !";
            connectionButton.Enabled = true;
        }

        public static void OnReceive(AlfredTask task)
        {
            ConnectionStatusDelegate d1 = ChangeConnectionStatus;
            form.Invoke(d1);

            if (task.Command != null && task.Type == TaskType.ActionPlayer)
                Launcher.ExecutePlayer(task);
        }

        public static void OnConnect()
        {
            alfredClient.Websocket.Player.Register(Settings.Default.ServerName);
        }

        public static void OnDisconnect()
        {
            ConnectionStatusDelegate d1 = ChangeConnectionStatus2;
            form.Invoke(d1);
        }
    }
}
