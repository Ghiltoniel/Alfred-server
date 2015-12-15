using System.Collections.Generic;
using System.Linq;
using Alfred.Model.Core;
using Alfred.Utils.Media;
using log4net;
using Newtonsoft.Json;

namespace Alfred.Utils.Managers
{
    public class PlayerManager
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PlayerManager));

        public static Client GetPlayer(string playerName, int channel)
        {
            var users = GetPlayersChannel(channel);
            return users.SingleOrDefault(u => u.Name == playerName && u.IsPlayer);
        }

        public static void SelectLeadingPlayer(int channel)
        {
            var remainingPlayers = GetPlayersChannel(channel);
            if (!remainingPlayers.Any())
            {
                //CommonPlugins.media.Stop();
                return;
            }

            var firstPlayer = remainingPlayers.First();
            firstPlayer.Send(new AlfredTask
            {
                Command = "Registered",
                Type = TaskType.ActionPlayer,
                Arguments = new Dictionary<string, string>
                {
                    {"isFirst", "True"}
                }
            });
            log.Info(string.Format("{0} : {1}", firstPlayer.Name, "Set as leading player"));
        }

        /// <summary>
        /// Broadcasts a message to all players for the selected channel
        /// </summary>
        /// <param name="message">Message to be broadcast</param>
        /// <param name="channel">the channel to cast to. -1 for all users</param>
        public static void BroadcastPlayersChannel(string message, int channel = 0)
        {
            var users = GetPlayersChannel(channel);
            foreach (var u in users)
            {
                u.Context.Send(message);
                log.Info(string.Format("Sent '{0}' to {1}:{2}",
                    message,
                    u.Context.ConnectionInfo.ClientIpAddress,
                    u.Context.ConnectionInfo.ClientPort));
            }
        }

        /// <summary>
        /// Broadcasts a message to all players for the selected channel
        /// </summary>
        /// <param name="task">The task object</param>
        /// <param name="channel">the channel to cast to. -1 for all users</param>
        public static void BroadcastPlayersChannel(AlfredTask task, int channel = 0)
        {
            var users = GetPlayersChannel(channel);
            if (users.Count() > 1)
                task.Arguments.Add("sync", "1");

            BroadcastPlayersChannel(JsonConvert.SerializeObject(task), channel);
        }

        public static IEnumerable<Client> GetPlayersChannel(int channel)
        {
           return Init.WebSocketServer.GetPlayersChannel(channel);
        }

        public static IEnumerable<Player> ToModelPlayer(IEnumerable<Client> clients)
        {
            var players = new HashSet<Player>();
            foreach (var client in clients)
            {
                players.Add(new Player
                {
                    Name = client.Name,
                    Channel = client.Channel
                });
            }
            return players;
        }
    }
}
