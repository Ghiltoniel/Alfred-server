using System.Collections.Generic;
using Alfred.Model.Core;
using Alfred.Model.Core.Music;
using Newtonsoft.Json;

namespace Alfred.Client.Core.Plugins
{
    public class MediaManager
    {
        public void AddToPlaylist(string artist, string album, string title, string file)
        {            
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_AddToPlaylist",
                Arguments = new Dictionary<string, string>
                {
                    { "artist", artist },
                    { "album", album },
                    { "title", title },
                    { "file", file }
                }
            });
        }

        public void AddAllToPlaylist(string genre, string artist, string album)
        {         
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_AddAllToPlaylist",
                Arguments = new Dictionary<string, string>
                {
                    { "artist", artist },
                    { "album", album },
                    { "genre", genre }
                }
            });
        }

        public void AddSongsToPlaylist(IEnumerable<Song> songs)
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_AddSongsToPlaylist",
                Arguments = new Dictionary<string, string>
                {
                    { "songs", JsonConvert.SerializeObject(songs) }
                }
            });
        }

        public void SetSongsPlaylist(IEnumerable<Song> songs)
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_SetSongsPlaylist",
                Arguments = new Dictionary<string, string>
                {
                    { "songs", JsonConvert.SerializeObject(songs) }
                }
            });
        }

        public void AddToPlaylistAndPlay(string artist, string album, string title, string file)
        {         
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_AddToPlaylistAndPlay",
                Arguments = new Dictionary<string, string>
                {
                    { "artist", artist },
                    { "album", album },
                    { "title", title },
                    { "file", file }
                }
            });
        }

        public void DirectPlay(string artist, string album, string title, string file)
        {         
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_DirectPlay",
                Arguments = new Dictionary<string, string>
                {
                    { "artist", artist },
                    { "album", album },
                    { "title", title },
                    { "file", file }
                }
            });
        }

        public void PlayPause()
        {      
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_PlayPause"
            });
        }

        public void Stop()
        {      
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_Stop"
            });
        }

        public void Next()
        {      
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_Next"
            });
        }

        public void Previous()
        {      
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_Previous"
            });
        }

        public void GoPlaylist(int index)
        {      
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_GoPlaylist",
                Arguments = new Dictionary<string, string>
                {
                    { "index", index.ToString() }
                }
            });
        }

        public void DeleteFromPlaylist(int index)
        {      
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_DeleteFromPlaylist",
                Arguments = new Dictionary<string, string>
                {
                    { "index", index.ToString() }
                }
            });
        }

        public void SetPosition(float position)
        {      
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_SetPosition",
                Arguments = new Dictionary<string, string>
                {
                    { "position", position.ToString() }
                }
            });
        }       

        public void PlaylistArtist(string artist)
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_PlaylistArtist",
                Arguments = new Dictionary<string, string>
                {
                    { "artist", artist }
                }
            });
        }

        public void PlaylistGenre(string genre)
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_PlaylistGenre",
                Arguments = new Dictionary<string, string>
                {
                    { "genre", genre }
                }
            });
        }

        public void DeleteAllFromPlaylist()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_DeleteAllFromPlaylist"
            });
        }
        
        public void UpdateStatus(int status, double length, double position, float? volume)
        {
            var task = new AlfredTask
            {
                Command = "MediaManager_UpdateStatus",
                Type = TaskType.Server
            };
            task.Arguments = new Dictionary<string, string>();
            task.Arguments.Add("status", status.ToString());
            task.Arguments.Add("length", length.ToString());
            task.Arguments.Add("position", position.ToString());
            if (volume.HasValue)
                task.Arguments.Add("volume", volume.ToString());

            AlfredPluginsWebsocket.Client.SendCommand(task);
        }

        public void Tournedisque()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_Tournedisque"
            });
        }

        public void RecognizeMusic()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_RecognizeMusic"
            });
        }

        public void StartContinuousPlaying()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_ContinuousPlaying",
                Type = TaskType.ActionPlayer
            });
        }

        public void StopContinuousPlaying()
        {
            AlfredPluginsWebsocket.Client.SendCommand(new AlfredTask
            {
                Command = "MediaManager_StopContinuousPlaying",
                Type = TaskType.ActionPlayer
            });
        }
    }
}
