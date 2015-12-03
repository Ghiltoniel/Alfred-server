using System.Collections.Generic;
using Alfred.Model.Core;

namespace Alfred.Client.Plugins
{
    public class Playlist
    {
        public void Launch(string name)
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Playlist_Launch",
                Arguments = new Dictionary<string, string>
                {
                    { "playlist", name }
                }
            });
        }

        public void AddCurrentMusicToPlaylist(string playlist)
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "Playlist_AddCurrentMusicToPlaylist",
                Arguments = new Dictionary<string, string>
                {
                    { "playlist", playlist }
                }
            });
        }
    }
}