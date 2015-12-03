using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;

namespace Alfred.Model.Db.Repositories
{
    public class MusicRepository
    {
        public IEnumerable<Music> GetByCriteria(string genre = null, string artist = null, string album = null, string title = null)
        {
            using (var db = new AlfredContext())
            {
                var musiques = db.Musics.AsQueryable();
                if (!string.IsNullOrEmpty(genre))
                    musiques = musiques.Where(m => string.Equals(m.Genre, genre));
                if (!string.IsNullOrEmpty(album))
                    musiques = musiques.Where(m => string.Equals(m.Album, album));
                if (!string.IsNullOrEmpty(artist))
                    musiques = musiques.Where(m => string.Equals(m.Artist, artist));
                if (!string.IsNullOrEmpty(title))
                    musiques = musiques.Where(m => string.Equals(m.Title, title));
                return musiques.ToList();
            }
        }

        public IEnumerable<string> GetAllArtists(string genre = null)
        {
            using (var db = new AlfredContext())
            {
                return db.Musics
                    .Where(m => genre == null || m.Genre == genre)
                    .Select(m => m.Artist)
                    .Distinct()
                    .OrderBy(a => a)
                    .ToList();
            }
        }

        public IEnumerable<string> GetAllAlbum(string artist = null)
        {
            using (var db = new AlfredContext())
            {
                return db.Musics
                    .Where(m => artist == null || m.Artist == artist)
                    .Select(m => m.Album)
                    .Distinct()
                    .OrderBy(a => a)
                    .ToList();
            }
        }

        public IEnumerable<string> GetAllGenres()
        {
            using (var db = new AlfredContext())
            {
                return db.Musics
                    .Select(m => m.Genre)
                    .Distinct()
                    .OrderBy(m => m)
                    .ToList();
            }
        }

        public IEnumerable<string> GetAllPlaylists()
        {
            using (var db = new AlfredContext())
            {
                return db.Playlists
                    .Select(p => p.Name)
                    .OrderBy(p => p).ToList();
            }
        }

        public IEnumerable<Music> GetMusicsFromPlaylist(string playlist)
        {
            using (var db = new AlfredContext())
            {
                return db.MusicPlaylists
                    .Include(mp => mp.Music)
                    .OrderBy(lk => lk.Order)
                    .Where(lk => lk.Playlist.Name == playlist)
                    .Select(lk => lk.Music)
                    .ToList();
            }
        }

        public void AddMusicToPlaylist(Music music, string playlistName)
        {
            using (var db = new AlfredContext())
            {
                var existingPlaylist = db.Playlists.SingleOrDefault(p => p.Name == playlistName);
                if (existingPlaylist != null)
                {
                    if (music != null)
                    {
                        if (db.Musics.SingleOrDefault(m => m.Path == music.Path) == null)
                        {
                            db.Musics.Add(music);
                            db.SaveChanges();
                        }

                        music = db.Musics.SingleOrDefault(m => m.Path == music.Path);
                        if (music != null)
                        {
                            var lks = existingPlaylist.Playlists;
                            var order = (lks.Count > 0) ? lks.Max(l => l.Order) + 1 : 0;
                            var newLk = new MusicPlaylist { Music_Id = music.Id, Playlist_Id = existingPlaylist.Id, Order = order };
                            existingPlaylist.Playlists.Add(newLk);

                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        public IEnumerable<Music> GetPlaylistByArtist(string name, int size)
        {
            using (var db = new AlfredContext())
            {
                var musics = db.Musics.Where(m => m.Artist == name).Take(size);
                return musics.ToList();
            }
        }

        public IEnumerable<Music> GetPlaylistByGenre(string name, int size)
        {
            using (var db = new AlfredContext())
            {
                var musics = db.Musics.Where(m => m.Genre == name).Take(size);
                return musics.ToList();
            }
        }
    }
}
