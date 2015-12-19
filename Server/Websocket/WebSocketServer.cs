using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Alfred.Model.Core;
using Alfred.Model.Core.WebSocket;
using Alfred.Utils.Managers;
using log4net;
using Newtonsoft.Json;
using Fleck2.Interfaces;
using System.Security.Cryptography.X509Certificates;

namespace Alfred.Server
{
    public class WebSocketServer : IAlfredWebSocketServer
    {
        private readonly ILog log = LogManager.GetLogger(typeof(WebSocketServer));

        /// <summary>
        /// Store the list of online users. Wish I had a ConcurrentList. 
        /// </summary>
        public readonly ConcurrentDictionary<Client, string> OnlineUsers = new ConcurrentDictionary<Client, string>();
        Fleck2.WebSocketServer _fServer;

        public void Start(string webSocketAddress)
        {
            _fServer = new Fleck2.WebSocketServer(webSocketAddress);
            try
            {
                _fServer.Start(socket =>
                    {
                        socket.OnOpen = () => OnConnect(socket);
                        socket.OnClose = () => OnDisconnect(socket);
                        socket.OnMessage = message => OnReceive(message, socket);
                    });
                log.Info(string.Format("WebSocket Server started at : {0}", webSocketAddress));
            }
            catch(SocketException e)
            {
                log.Error(e.Message);
            }
        }

        void OnReceive(string command, Fleck2.Interfaces.IWebSocketConnection socket)
        {
            if (command.Length < 4)
                return;
            try
            {
                switch (command)
                {
                    case "ping":
                        socket.Send("pong");
                        log.Info(string.Format("{0}:{1} : Sending Pong", socket.ConnectionInfo.ClientIpAddress, socket.ConnectionInfo.ClientPort));
                        break;
                    default:
                        var task = JsonConvert.DeserializeObject<AlfredTask>(command);
                        var u = OnlineUsers.Keys.Single(o => o.Context == socket);

                        log.Info(string.Format("{0} : {1} : Executing ...", task.FromName, task.Command));
                        task.Client = u;
                        Launcher.EnqueueTask(task);
                        break;
                }
            }
            catch (Exception e) // Bad JSON! For shame.
            {
                log.ErrorFormat("Error when executing task : {0}, error: {1}", command, e.Message);
            }
        }

        /// <summary>
        /// Event fired when a client disconnects from the Alchemy Websockets server instance.
        /// Removes the user from the online users list and broadcasts the disconnection message
        /// to all connected users.
        /// </summary>
        /// <param name="socket">The user's connection context</param>
        void OnDisconnect(IWebSocketConnection socket)
        {
            var user = OnlineUsers.Keys.SingleOrDefault(o => o.Context == socket);
            if (user == null)
                return;

            string trash; // Concurrent dictionaries make things weird
            OnlineUsers.TryRemove(user, out trash);

            if (!String.IsNullOrEmpty(user.Name))
            {
                log.Info(string.Format("{0} : {1}", user.Name, "Disconnect"));
                Init.serverStatus.Players.RemoveAll(p => p.Name == user.Name);
                UpdateServerStatus();
                log.Info(string.Format("{0} : {1}", user.Name, "Unregister"));

                PlayerManager.SelectLeadingPlayer(user.Channel);
                Broadcast(new AlfredTask
                {
                    Command = "UpdatePlayers",
                    Type = TaskType.WebsiteInfo,
                    Arguments = new Dictionary<string, string> { { "players", JsonConvert.SerializeObject(PlayerManager.ToModelPlayer(PlayerManager.GetPlayersChannel(-1))) } }
                });
            }
        }

        void UpdateServerStatus()
        {
            var task = new AlfredTask
            {
                Type = TaskType.WebsiteInfo,
                Command = "UpdateServerStatus",
                Arguments = new Dictionary<string, string>
                {
                    { "status", JsonConvert.SerializeObject(Init.serverStatus) }
                }
            };

            Broadcast(task);
        }

        void OnConnect(IWebSocketConnection socket)
        {
            var me = new Client {
                Context = socket,
                Channel = 0
            };

            OnlineUsers.TryAdd(me, String.Empty);
            log.Info(string.Format("{0} : {1}", socket.ConnectionInfo.ClientIpAddress + ":" + socket.ConnectionInfo.ClientPort, "Connect"));
        }


        /// <summary>
        /// Broadcasts a message to all users, or if users is populated, a select list of users
        /// </summary>
        public void Broadcast(AlfredTask task)
        {
            Broadcast(JsonConvert.SerializeObject(task));
        }

        /// <summary>
        /// Broadcasts a message to all users, or if users is populated, a select list of users
        /// </summary>
        /// <param name="message">Message to be broadcast</param>
        public void Broadcast(string message)
        {
            var users = OnlineUsers.Keys;
            foreach (var u in users)
            {
                u.Context.Send(message);
                log.Info(string.Format("Sent '{0}' to {1}:{2}",
                    message,
                    u.Context.ConnectionInfo.ClientIpAddress,
                    u.Context.ConnectionInfo.ClientPort));
            }
        }

        public IEnumerable<Client> GetPlayersChannel(int channel)
        {
            var usersValid = OnlineUsers.Keys.Where(u => u.IsPlayer);
            var users = (channel != -1) ? usersValid.Where(u => u.Channel == channel) : usersValid;
            return users;
        }
    }
}
