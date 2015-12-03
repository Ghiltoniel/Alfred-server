using System.Collections.Generic;
using System.Linq;
using Alfred.Model.Db;
using Alfred.Model.Db.Repositories;

namespace Alfred.Utils
{
    public class PlaylistManager
	{
        private MusicRepository _musicRepo;
		public int sizePlaylist { get; set; }

		public PlaylistManager()
		{
            sizePlaylist = 15;
            _musicRepo = new MusicRepository();
		}

        public List<Music> PlaylistArtist(string artist)
        {
            var songs = _musicRepo.GetPlaylistByArtist(artist, sizePlaylist).ToList();
            songs.Shuffle();
            return songs;
        }

        public List<Music> PlaylistGenre(string genre)
        {
            var songs = _musicRepo.GetPlaylistByGenre(genre, sizePlaylist).ToList();
            songs.Shuffle();
            return songs;
        }
	}
}
