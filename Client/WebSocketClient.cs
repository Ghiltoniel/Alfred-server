using System;
using System.Collections.Generic;
using Alfred.Model.Core;
using Newtonsoft.Json;
using WebSocket4Net;
using System.Threading.Tasks;

namespace Alfred.Client
{
    public class WebSocketClient
    {
        protected string ws_server;
        protected string host;
        protected int port = 13100;
        protected string clientName;
        protected string login;
        protected string password;
        protected string token;
        protected WebSocket client;
        protected Action<AlfredTask> _onReceive;
        protected Action _onDisconnect;
        protected Action _onConnect;
        public bool IsConnected;
        private TaskCompletionSource<double> _tcs;
        private DateTime _pingDate;

        public WebSocketClient(
            string clientName, 
            string host, 
            int port, 
            string login,
            string password,
            Action<AlfredTask> onReceive = null,
            Action onDisconnect = null,
            Action onConnect = null)
        {
            this.host = host;
            this.port = port;
            this.login = login;
            this.password = password;
            this._onReceive = onReceive;
            this._onDisconnect += onDisconnect;
            this._onConnect += onConnect;

            ws_server = string.Format("ws://{0}:{1}/{2}", this.host, this.port, "channel");
            this.clientName = clientName;
        }

        public void Connect()
        {
            try
            {
                if (client == null
                    || client.State == WebSocketState.Closed
                    || client.State == WebSocketState.Closing
                    || client.State == WebSocketState.None)
                {
                    client = new WebSocket(ws_server);
                    client.MessageReceived += WebsocketReceived;

                    client.Opened += client_Opened;
                    client.Closed += client_Closed;
                    client.Open();
                    IsConnected = true;
                }
            }
            catch (Exception)
            {
                
            }
        }

        public void WebsocketReceived(object sender, MessageReceivedEventArgs e)
        {
            try
            {
                if(e.Message == "pong")
                    _tcs.TrySetResult((DateTime.Now - _pingDate).TotalMilliseconds);

                var task = JsonConvert.DeserializeObject<AlfredTask>(e.Message);
                if (task.Command == "Authenticated")
                {
                    token = task.Arguments["token"];
                    AlfredClient.Token = token;
                    if (_onConnect != null)
                        _onConnect();
                }
                else
                {
                    if (_onReceive != null)
                        _onReceive(task);
                }
            }
            catch (Exception)
            {
            }
        }

        void client_Closed(object sender, EventArgs e)
        {
            if (_onDisconnect != null)
                _onDisconnect();

            IsConnected = false;
        }

        void client_Opened(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(token))
                Login(login, password);

            IsConnected = true;
        }

        public void SendCommand(string message)
        {
            if (client == null || client.State != WebSocketState.Open)
                Connect();

            if (IsConnected)
            {
                SendMessageTcp(message);
            }
        }

        public void SendCommand(AlfredTask task)
        {
            if(!string.IsNullOrEmpty(token))
            {
                if (task.Arguments == null)
                    task.Arguments = new Dictionary<string, string>();

                task.Arguments.Add("token", token);
            }
            SendCommand(JsonConvert.SerializeObject(task));
        }

        private void SendMessageTcp(string message)
        {
            try
            {
                if (client.State == WebSocketState.Open)
                    client.Send(message);
            }
            catch (Exception) { }
        }

        public void Login(string login, string password)
        {
            SendCommand(new AlfredTask
            {
                Command = "User_Login",
                Arguments = new Dictionary<string, string>
                {
                    { "login", login },
                    { "password", password }
                }
            });
        }

        public Task<double> Ping()
        {            
            _tcs = null;
            _pingDate = DateTime.Now;
            this.SendCommand("ping");
            return _tcs.Task;
        }

    }
}