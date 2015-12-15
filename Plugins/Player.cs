using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Alfred.Model.Core;
using Alfred.Plugins.Managers;
using Alfred.Utils.Managers;
using Alfred.Utils.Plugins;
using Newtonsoft.Json;

namespace Alfred.Plugins
{
    public class Player : BasePlugin
    {
        static Timer _synchronizeTimeout = new Timer(4000);

        public override void Initialize()
        {

        }

        public void Register()
        {
            var name = arguments["name"];
            var playersChannel = PlayerManager.GetPlayersChannel(client.Channel);

            var isFirst = !playersChannel.Any();
            client.Name = name;
            client.IsPlayer = true;

            client.Send(new AlfredTask
            {
                Command = "Registered",
                Type = TaskType.ActionPlayer,
                Arguments = new Dictionary<string, string>
                {
                    {"isFirst", isFirst.ToString()}
                }
            });

            UpdateWebsitePlayers();
            CommonPlugins.media.SynchronizePlayers();
        }


        /// <summary>
        /// Unregister a player (remove his name
        /// </summary>
        public void Unregister()
        {
            client.Send(new AlfredTask
            {
                Command = "Unregistered"
            });
            client.Name = string.Empty;
            client.IsPlayer = false;

            PlayerManager.SelectLeadingPlayer(client.Channel);
            UpdateWebsitePlayers();
        }

        public void UpdateWebsitePlayers()
        {
            WebSocketServer.Broadcast(new AlfredTask
            {
                Command = "UpdatePlayers",
                Type = TaskType.WebsiteInfo,
                Arguments = new Dictionary<string, string> { { "players", JsonConvert.SerializeObject(PlayerManager.ToModelPlayer(PlayerManager.GetPlayersChannel(-1))) } }
            });
        }

        public void ReadyToPlay()
        {
            if (client != null)
                client.ReadyToPlay = true;

            var channel = client.Channel;

            var usersChannel = PlayerManager.GetPlayersChannel(channel);
            if (usersChannel.All(us => us.ReadyToPlay))
                Synchronized(channel);
            else
            {
                _synchronizeTimeout.Stop();
                //_synchronizeTimeout.Elapsed += (object sender, ElapsedEventArgs e) => Synchronized(channel);
                // _synchronizeTimeout.Start();
            }
        }

        public void Synchronized(int channel = 0)
        {
            _synchronizeTimeout.Stop();
            var users = PlayerManager.GetPlayersChannel(channel);
            foreach (var u in users)
            {
                u.Context.Send("play");
                u.ReadyToPlay = false;
            }
        }
    }
}
