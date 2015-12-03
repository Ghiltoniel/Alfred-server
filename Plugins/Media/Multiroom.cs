using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Alfred.Model.Core;
using Alfred.Plugins.Managers;
using Alfred.Utils.Managers;
using Alfred.Utils.Plugins;

namespace Alfred.Plugins
{
    public class Multiroom : BasePlugin
    {
        public void SynchronizePlayer()
        {
            var playerName = arguments["name"];
            var channel = int.Parse(arguments["channel"]);
            var user = PlayerManager.GetPlayer(playerName, channel);
            if (user != null)
                user.Channel = channel;

            SynchronizePlayers();
        }

        public void SynchronizePlayers()
        {
            var channel = int.Parse(arguments["channel"]);
            var task = new AlfredTask
            {
                Command = "Synchronize",
                Type = TaskType.ActionPlayer,
                Arguments = new Dictionary<string, string>()
            };

            var status = CommonPlugins.media.Statuts[channel];
            var playlist = CommonPlugins.media.Playlists[channel];

            task.Arguments.Add("pos", status.Position.ToString(CultureInfo.InvariantCulture));
            task.Arguments.Add("status", status.Status.ToString(CultureInfo.InvariantCulture));
            if (playlist.ContainsKey(status.CurrentPlaylistIndex))
                task.Arguments.Add("path", playlist[status.CurrentPlaylistIndex].Path);

            var usersChannel = PlayerManager.GetPlayersChannel(channel);
            if (usersChannel.Count() > 1)
                PlayerManager.BroadcastPlayersChannel(task, channel);
        }
    }
}
