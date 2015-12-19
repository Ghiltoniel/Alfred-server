using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Timers;
using Alfred.Model.Core;
using Alfred.Model.Core.Music;
using Alfred.Utils;
using Alfred.Model.Db;
using Alfred.Utils.Managers;
using Alfred.Utils.Plugins;
using Newtonsoft.Json;
using Alfred.Model.Db.Repositories;

namespace Alfred.Plugins
{
    public class MediaManager : BasePlugin
    {
        private readonly int[] _inactiveState = { -1, 0, 4, 5 };
        private readonly Dictionary<int, Timer> _positionTimers;
        private MusicRepository _musicRepo;

        public int CurrentChannel;
        public Dictionary<int, SortedList<int, Music>> Playlists;
        public Dictionary<int, PlayerStatus> Statuts;

        public MediaManager()
        {
            Statuts = new Dictionary<int, PlayerStatus> {{0, new PlayerStatus(0)}};
            Playlists = new Dictionary<int, SortedList<int, Music>>();
            _positionTimers = new Dictionary<int, Timer> {{0, new Timer()}};
            _musicRepo = new MusicRepository();
        }

        #region Player remote
        public void GetPlaylistChannel(int channel)
        {
            if (!Playlists.ContainsKey(channel))
                Playlists.Add(channel, new SortedList<int, Music>());
        }

        public void GetStatusChannel(int channel)
        {
            if (!Statuts.ContainsKey(channel))
                Statuts.Add(channel, new PlayerStatus(channel));
        }

        public void GetPositionTimerChannel(int channel)
        {
            if (!_positionTimers.ContainsKey(channel))
            {
                var timer = new Timer(4000);
                timer.Elapsed += positionTimer_Elapsed;
                _positionTimers.Add(channel, timer);
            }
        }

        public void LoadObjectsFromArguments(Dictionary<string, string> arguments)
        {
            CurrentChannel = (arguments != null && arguments.ContainsKey("channel")) ? int.Parse(arguments["channel"]) : 0;
            GetPlaylistChannel(CurrentChannel);
            GetStatusChannel(CurrentChannel);
            GetPositionTimerChannel(CurrentChannel);
        }

        public void AddToPlaylist()
        {
            var indexPlaylist = 0;
            if (Playlists[CurrentChannel].Keys.Count > 0)
                indexPlaylist = Playlists[CurrentChannel].Keys.Max() + 1;

            Playlists[CurrentChannel].Add(indexPlaylist, new Music {Artist = arguments["artist"], Album = arguments["album"], Title = arguments["title"], Path = arguments["file"] });
            UpdateWebsite();

            if (_inactiveState.Contains(Statuts[CurrentChannel].Status))
                PlaySongIndex(indexPlaylist);
        }

        public void AddAllToPlaylist()
        {
            var indexPlaylist = 0;
            var i = 0;
            if (Playlists[CurrentChannel].Keys.Count > 0)
            {
                indexPlaylist = Playlists[CurrentChannel].Keys.Max() + 1;
                i = indexPlaylist;
            }

            var genre = arguments.ContainsKey("genre") ? arguments["genre"] : "";
            var album = arguments.ContainsKey("album") ? arguments["album"] : "";
            var artist = arguments.ContainsKey("artist") ? arguments["artist"] : "";

            var count = int.Parse(arguments.ContainsKey("count") ? arguments["count"] : "15");

            var songs = _musicRepo.GetByCriteria(genre, artist, album);
            var songsList = songs.OrderBy(m => m.Title).Take(count).ToList();
            songsList.Shuffle();

            foreach (var song in songsList)
            {
                Playlists[CurrentChannel].Add(i, new Music {Artist = song.Artist, Album = song.Album, Title = song.Title, Path = song.Path });
                i++;
            }
            UpdateWebsite();

            if (_inactiveState.Contains(Statuts[CurrentChannel].Status))
                PlaySongIndex(indexPlaylist);
        }

        public void BroadcastStatus()
        {
            UpdateWebsite();
        }

        public void AddSongsToPlaylist()
        {
            var indexPlaylist = 0;
            var i = 0;
            if (Playlists[CurrentChannel].Keys.Count > 0)
            {
                indexPlaylist = Playlists[CurrentChannel].Keys.Max() + 1;
                i = indexPlaylist;
            }

            var songs = JsonConvert.DeserializeObject<IEnumerable<Song>>(arguments["songs"]);

            foreach (var song in songs)
            {
                Playlists[CurrentChannel].Add(i, new Music {Artist = song.Artist, Album = song.Album, Title = song.Title, Path = song.Link });
                i++;
            }
            UpdateWebsite();

            if (_inactiveState.Contains(Statuts[CurrentChannel].Status))
                PlaySongIndex(indexPlaylist);
        }

        public void SetSongsPlaylist()
        {
            Playlists[CurrentChannel] = new SortedList<int, Music>();
            var songs = JsonConvert.DeserializeObject<IEnumerable<Song>>(arguments["songs"]);

            var i = 0;
            foreach (var song in songs)
            {
                Playlists[CurrentChannel].Add(i, new Music {Artist = song.Artist, Album = song.Album, Title = song.Title, Path = song.Link });
                i++;
            }
            UpdateWebsite();
            PlaySongIndex(0);
        }

        public void AddToPlaylistAndPlay()
        {
            var indexPlaylist = 0;
            var maxIndex = Playlists[CurrentChannel].Keys.Count == 0 ? -1 : Playlists[CurrentChannel].Keys.Max();
            if (maxIndex >= 0)
                indexPlaylist = maxIndex + 1;

            Playlists[CurrentChannel].Add(indexPlaylist, new Music {Artist = arguments["artist"], Album = arguments["album"], Title = arguments["title"], Path = arguments["file"] });
            UpdateWebsite();
            PlaySongIndex(maxIndex + 1);
        }

        public void DirectPlay()
        {
            Playlists[CurrentChannel] = new SortedList<int, Music>();

            Playlists[CurrentChannel].Add(0, new Music {Artist = arguments["artist"], Album = arguments["album"], Title = arguments["title"], Path = arguments["file"] });
            UpdateWebsite();
            PlaySongIndex(0);
        }

        public void PlayPause()
        {
            var task = new AlfredTask
            {
                Command = "PlayPause",
                Type = TaskType.ActionPlayer
            };
            PlayerManager.BroadcastPlayersChannel(task, CurrentChannel);
        }

        public void Stop()
        {
            Statuts[CurrentChannel].Position = 0;
            Statuts[CurrentChannel].Status = -1;
            _positionTimers[CurrentChannel].Enabled = false;
            var task = new AlfredTask
            {
                Command = "Stop",
                Type = TaskType.ActionPlayer
            };
            PlayerManager.BroadcastPlayersChannel(task, CurrentChannel);
        }

        public void Next()
        {
            var nextIndexes = Playlists[CurrentChannel].Keys.Where(k => k > Statuts[CurrentChannel].CurrentPlaylistIndex);
            
            if(!nextIndexes.Any())
            {
                if (!Playlists[CurrentChannel].Any())
                    return;

                var currentSong = Playlists[CurrentChannel][Statuts[CurrentChannel].CurrentPlaylistIndex];
                var i = Statuts[CurrentChannel].CurrentPlaylistIndex + 1;
                var songs = _musicRepo.GetByCriteria(currentSong.Genre, currentSong.Artist, currentSong.Album);
                var songsList = songs
                    .Where(s=>s.Path != currentSong.Path)
                    .OrderBy(m => m.Title)
                    .Take(5)
                    .ToList();

                songsList.Shuffle();

                foreach (var song in songsList)
                {
                    Playlists[CurrentChannel].Add(i, new Music {Artist = song.Artist, Album = song.Album, Title = song.Title, Path = song.Path });
                    i++;
                }
                UpdateWebsite();
            }
            if (nextIndexes.Any())
                PlaySongIndex(nextIndexes.Min());
        }

        public void Previous()
        {
            var previousIndexes = Playlists[CurrentChannel].Keys.Where(k => k < Statuts[CurrentChannel].CurrentPlaylistIndex);
            var enumerable = previousIndexes as int[] ?? previousIndexes.ToArray();
            if (enumerable.Any())
                PlaySongIndex(enumerable.Max());
        }

        public void GoPlaylist()
        {
            PlaySongIndex(int.Parse(arguments["index"]));
        }

        public void DeleteFromPlaylist()
        {
            var index = int.Parse(arguments["index"]);
            var song = Playlists[CurrentChannel].SingleOrDefault(p => p.Key == index);
            if (song.Value != null)
            {
                Playlists[CurrentChannel].Remove(index);
                if (index == Statuts[CurrentChannel].CurrentPlaylistIndex)
                {
                    Stop();
                    Next();
                }
                UpdateWebsite();
            }
        }

        public void PlaySongIndex(int i)
        {
            Music song;
            var songExists = Playlists[CurrentChannel].TryGetValue(i, out song);

            if (songExists)
            {
                UpdateStatus(null, 0, null, null, i);
                Statuts[CurrentChannel].CurrentPlaylistIndex = i;
                Statuts[CurrentChannel].Position = 0;

                var task = new AlfredTask
                {
                    Command = "DirectPlay",
                    Type = TaskType.ActionPlayer
                };
                task.Arguments = new Dictionary<string, string>();
                task.Arguments.Add("file", song.Path);
                PlayerManager.BroadcastPlayersChannel(task, CurrentChannel);
            }
            else
            {
                Statuts[CurrentChannel].Status = -1;
            }
            UpdateWebsite();
        }

        public void SetPosition()
        {
            Statuts[CurrentChannel].Position = int.Parse(arguments["position"]) * Statuts[CurrentChannel].Length / 100;

            var task = new AlfredTask
            {
                Command = "SetPosition",
                Type = TaskType.ActionPlayer
            };
            task.Arguments = new Dictionary<string, string>();
            task.Arguments.Add("position", (Statuts[CurrentChannel].Position).ToString(CultureInfo.InvariantCulture));
            PlayerManager.BroadcastPlayersChannel(task, CurrentChannel);
        }
        #endregion

        #region Playlist CRUD

        public void CreatePlaylistFromSongs(IEnumerable<Music> songs)
        {
            var indexPlaylist = 0;
            Playlists[CurrentChannel] = new SortedList<int, Music>();

            foreach (var song in songs)
            {
                Playlists[CurrentChannel].Add(indexPlaylist, new Music
                {
                    Album = song.Album,
                    Artist = song.Artist,
                    Genre = song.Genre,
                    Path = song.Path,
                    Title = song.Title
                });
                indexPlaylist++;
            }
            UpdateWebsite();
            PlaySongIndex(0);
        }

        public void PlaylistArtist()
        {
            var playlist = new PlaylistManager();
            var songs = playlist.PlaylistArtist(arguments["artist"]);
            CreatePlaylistFromSongs(songs);
        }

        public void PlaylistGenre()
        {
            var playlist = new PlaylistManager();
            var songs = playlist.PlaylistGenre(arguments["genre"]);
            CreatePlaylistFromSongs(songs);
        }

        public void DeleteAllFromPlaylist()
        {
            if (Statuts[CurrentChannel].Status == 3)
            {
                var keys = Playlists[CurrentChannel].Keys.ToList();
                foreach (var ind in keys)
                {
                    if (ind != Statuts[CurrentChannel].CurrentPlaylistIndex)
                        Playlists[CurrentChannel].Remove(ind);
                }
            }
            else
                Playlists[CurrentChannel].Clear();

            UpdateWebsite();
        }
        #endregion

        #region Media update
        public void UpdateStatus()
        {
            double? length = null, position = null;
            float? volume = null;
            int? status = null;

            var culture = new CultureInfo("en-US");
            if (arguments.ContainsKey("length"))
                length = double.Parse(arguments["length"].Replace(",", "."), culture);

            if (arguments.ContainsKey("position"))
                position = double.Parse(arguments["position"].Replace(",", "."), culture);

            if (arguments.ContainsKey("volume"))
                volume = float.Parse(arguments["volume"].Replace(",", "."), culture);

            if (arguments.ContainsKey("status"))
                status = int.Parse(arguments["status"]);

            UpdateStatus(length, position, volume, status, null);
            UpdateWebsite();
        }

        public void UpdateStatus(double? length, double? position, float? volume, int? status, int? currentPlaylistIndex)
        {
            if (length.HasValue)
                Statuts[CurrentChannel].Length = length.Value;

            if (position.HasValue)
                Statuts[CurrentChannel].Position = position.Value;

            if (volume.HasValue)
                Statuts[CurrentChannel].Volume = volume.Value;

            if (currentPlaylistIndex.HasValue)
                Statuts[CurrentChannel].CurrentPlaylistIndex = currentPlaylistIndex.Value;

            if (status.HasValue)
            {
                Statuts[CurrentChannel].Status = status.Value;
                if (Statuts[CurrentChannel].Status == 3)
                    _positionTimers[CurrentChannel].Enabled = true;
                else
                    _positionTimers[CurrentChannel].Enabled = false;
            }
        }
        #endregion

        #region Send Updates to Website
        public void UpdateWebsite()
        {
            var task = new AlfredTask
            {
                Type = TaskType.WebsiteInfo,
                Command = "UpdateStatus",
                Arguments = new Dictionary<string, string>
                {
                    { "status", JsonConvert.SerializeObject(Statuts[CurrentChannel]) },
                    { "playlist", JsonConvert.SerializeObject(Playlists[CurrentChannel]) }
                }
            };

            WebSocketServer.Broadcast(task);
        }

        public void SynchronizePlayers()
        {
            if(!Playlists.ContainsKey(CurrentChannel))
            {
                Playlists.Add(CurrentChannel, new SortedList<int, Music>());
            }

            if (!Playlists[CurrentChannel].ContainsKey(Statuts[CurrentChannel].CurrentPlaylistIndex))
                return;

            var task = new AlfredTask
            {
                Type = TaskType.ActionPlayer,
                Command = "DirectPlay",
                Arguments = new Dictionary<string, string>
                {
                    { "position", Statuts[CurrentChannel].Position.ToString(CultureInfo.InvariantCulture) },
                    { "file", Playlists[CurrentChannel][Statuts[CurrentChannel].CurrentPlaylistIndex].Path }
                }
            };
            PlayerManager.BroadcastPlayersChannel(task, CurrentChannel);
        }
        #endregion

        #region Tournedisque

        public void Tournedisque()
        {
            arguments["artist"] = "Tournedisque";
            arguments["album"] = "";
            arguments["title"] = "mix";
            arguments["file"] = "Tournedisque";
            DirectPlay();
        }
        #endregion

        public void RecognizeMusic()
        {
            //new Recorder().RecordAudio(20);
        }

        void positionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var status = Statuts[CurrentChannel];
            UpdateStatus(status.Length, status.Position + 1, status.Volume, status.Status, status.CurrentPlaylistIndex);
            UpdateWebsite();
        }
    }
}
