using Alfred.Model.Db.Repositories;
using Alfred.Plugins.Managers;
using Alfred.Utils;
using Alfred.Utils.Plugins;

namespace Alfred.Plugins
{
    public class Playlist : BasePlugin
    {
        private MusicRepository _musicRepo;

        public Playlist()
        {
            _musicRepo = new MusicRepository();
        }

        public void Launch()
        {
            var musics = _musicRepo.GetMusicsFromPlaylist(arguments["playlist"]);
            CommonPlugins.media.CreatePlaylistFromSongs(musics);
        }

        public void AddCurrentMusicToPlaylist()
        {
            var musique =
                CommonPlugins.media.Playlists[CommonPlugins.media.CurrentChannel][
                    CommonPlugins.media.Statuts[CommonPlugins.media.CurrentChannel].CurrentPlaylistIndex];
            _musicRepo.AddMusicToPlaylist(musique, arguments["playlist"]);
        }
    }
}