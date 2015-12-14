using System;
using Alfred.Model.Core;
using WebSocket4Net;
using System.Threading.Tasks;

namespace Alfred.Client.Core
{
    public class AlfredClient
    {
        public AlfredPluginsWebsocket Websocket { get; private set; }
        public AlfredPluginsHttp Http { get; private set; }
        private WebSocketClient WsClient;

        public bool IsConnected
        {
            get
            {
                return WsClient.IsConnected;
            }
        }

        public AlfredClient(
            string name, 
            string host, 
            int port,
            string username, 
            string password,
            Action<AlfredTask> receiveDelegate = null,
            Action disconnectDelegate = null,            
            Action connectDelegate = null
            )
        {
            WsClient = new WebSocketClient(name, host, port, username, password, receiveDelegate, disconnectDelegate, connectDelegate);

            try {
                WsClient.Connect();
            }
            catch(Exception e)
            {

            }

            Websocket = new AlfredPluginsWebsocket(WsClient);
            Http = new AlfredPluginsHttp();
        }

        public void Connect()
        {
            if(!WsClient.IsConnected)
                WsClient.Connect();
        }

        public async Task<double> Ping()
        {
            return await WsClient.Ping();
        }

        public void SendCommand(AlfredTask task)
        {
            WsClient.SendCommand(task);
        }

        public static string Token;
    }
}
