using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Alfred.Model.Db.Repositories
{
    public class MediaRepository
    {
        public bool MediaExists(string filePath)
        {
            using (var db = new AlfredContext())
            {
                return db.Musics.Any(m => m.Path == filePath) || db.Movies.Any(m => m.Path == filePath) || db.Episodes.Any(m => m.Path == filePath);
            }
        }

        public void SaveMovie(Movie movie)
        {
            using (var db = new AlfredContext())
            {
                db.Movies.Add(movie);
                db.SaveChanges();
            }
        }

        public void SaveEpisode(Episode episode)
        {
            using (var db = new AlfredContext())
            {
                db.Episodes.Add(episode);
                db.SaveChanges();
            }
        }

        public void SaveMusic(Music music)
        {
            using (var db = new AlfredContext())
            {
                db.Musics.Add(music);
                db.SaveChanges();
            }
        }


        public void SaveFromFiles(string[] audioExt, string[] movieExt)
        {
            using (var db = new AlfredContext())
            {
                var paths = db.Paths.ToDictionary(p => p.Name, p => p.Value);
                var musiquesFile = new List<Music>();
                var filmsFile = new List<Movie>();
                var episodesFile = new List<Episode>();

                if (paths.ContainsKey("Music"))
                {
                    var musiques = db.Musics.ToList();
                    var dir = new DirectoryInfo(paths["Music"]);

                    foreach (var genreDir in dir.GetDirectories())
                    {
                        var artists = genreDir.GetDirectories();
                        foreach (var artist in artists)
                        {
                            var albums = artist.GetDirectories();
                            foreach (var album in albums)
                            {
                                foreach (var song in album.GetFiles())
                                {
                                    if (audioExt.Contains(song.Extension))
                                    {
                                        var newMusique = new Music { Genre = genreDir.Name, Artist = artist.Name, Album = album.Name, Title = song.Name, Path = song.FullName };
                                        musiquesFile.Add(newMusique);
                                    }
                                }
                            }

                            foreach (var song in artist.GetFiles())
                            {
                                if (audioExt.Contains(song.Extension))
                                {
                                    var newMusique = new Music { Genre = genreDir.Name, Artist = artist.Name, Album = "", Title = song.Name, Path = song.FullName };
                                    musiquesFile.Add(newMusique);
                                }
                            }
                        }
                    }

                    var musicFileString = musiquesFile.Select(m => m.Path).ToList();
                    var musicDbString = musiques.Select(m => m.Path).ToList();
                    var deletedMusicsId = musiques.Where(m => !musicFileString.Contains(m.Path)).Select(m => m.Id);
                    var newMusics = musiquesFile.Where(m => !musicDbString.Contains(m.Path));

                    if (deletedMusicsId.Any())
                    {
                        var deletedIdsString = string.Join(", ", deletedMusicsId.ToArray());
                        var query = string.Format("delete from lk_musique_playlist where id_musique in ({0})", deletedIdsString);
                        db.Database.ExecuteSqlCommand(query);

                        query = string.Format("delete from musique where id in ({0}) and genre != 'grooveshark'", deletedIdsString);
                        db.Database.ExecuteSqlCommand(query);
                    }

                    foreach (var newMusic in newMusics)
                    {
                        db.Musics.Add(newMusic);
                    }
                }

                if (paths.ContainsKey("Movies"))
                {
                    var films = db.Movies;
                    var dir = new DirectoryInfo(paths["Movies"]);
                    foreach (var genre_dir in dir.GetDirectories())
                    {
                        var filmPaths = genre_dir.GetFiles("*.avi", SearchOption.AllDirectories).ToList();
                        filmPaths.AddRange(genre_dir.GetFiles("*.mkv", SearchOption.AllDirectories));
                        filmPaths.AddRange(genre_dir.GetFiles("*.m4a", SearchOption.AllDirectories));
                        filmPaths.AddRange(genre_dir.GetFiles("*.mp4", SearchOption.AllDirectories));
                        filmPaths.AddRange(genre_dir.GetFiles("*.m4p", SearchOption.AllDirectories));
                        filmPaths.AddRange(genre_dir.GetFiles("*.mov", SearchOption.AllDirectories));
                        filmPaths.AddRange(genre_dir.GetFiles("*.wmv", SearchOption.AllDirectories));

                        foreach (var film in filmPaths)
                        {
                            var newFilm = new Movie { Genre = genre_dir.Name, Title = film.Name, Path = film.FullName };
                            filmsFile.Add(newFilm);
                        }
                    }

                    var filmFileString = filmsFile.Select(m => m.Path);
                    var filmDbString = films.Select(m => m.Path);
                    var filmMusicsId = films.Where(m => !filmFileString.Contains(m.Path)).Select(m => m.Id);
                    var deletedFilmsId = films.Where(m => !filmFileString.Contains(m.Path)).Select(m => m.Id);
                    var updatedFilmsPath = films.Where(m => filmFileString.Contains(m.Path)).Select(m => m.Path);
                    var newFilms = filmsFile.Where(m => !filmDbString.Contains(m.Path));

                    if (deletedFilmsId.Any())
                    {
                        var deletedIdsString = string.Join(", ", deletedFilmsId.ToArray());
                        var query = string.Format("delete from film where id in ({0})", deletedIdsString);
                        db.Database.ExecuteSqlCommand(query);
                    }

                    foreach (var newFilm in newFilms)
                    {
                        db.Movies.Add(newFilm);
                    }
                }

                if (paths.ContainsKey("Series"))
                {
                    var episodes = db.Episodes;
                    var dir = new DirectoryInfo(paths["Series"]);
                    foreach (var name_dir in dir.GetDirectories())
                    {
                        var seasons = name_dir.GetDirectories();
                        foreach (var season in seasons)
                        {
                            foreach (var episode in season.GetFiles())
                            {
                                if (movieExt.Contains(episode.Extension))
                                {
                                    byte resultString;
                                    var isParsed = byte.TryParse(Regex.Match(season.Name, @"\d+").Value, out resultString);

                                    if (isParsed)
                                    {
                                        var newEpisode = new Episode { ShowName = name_dir.Name, SeasonNumber = resultString, Path = episode.FullName };
                                        episodesFile.Add(newEpisode);
                                    }
                                }
                            }
                        }
                        foreach (var episode in name_dir.GetFiles())
                        {
                            if (movieExt.Contains(episode.Extension))
                            {
                                var newEpisode = new Episode { ShowName = name_dir.Name, SeasonNumber = 1, Path = episode.FullName };
                                episodesFile.Add(newEpisode);
                            }
                        }
                    }

                    var episodeFileString = episodesFile.Select(m => m.Path).ToList();
                    var episodeDbString = episodes.Select(m => m.Path).ToList();
                    var deletedEpisodesId = episodes.Where(m => !episodeFileString.Contains(m.Path)).Select(m => m.Id);
                    var updatedEpisodesPath = episodes.Where(m => episodeFileString.Contains(m.Path)).Select(m => m.Path);
                    var newEpisodes = episodesFile.Where(m => !episodeDbString.Contains(m.Path));

                    if (deletedEpisodesId.Any())
                    {
                        var deletedIdsString = string.Join(", ", deletedEpisodesId.ToArray());
                        var query = string.Format("delete from episode where id in ({0})", deletedIdsString);
                        db.Database.ExecuteSqlCommand(query);
                    }

                    foreach (var newEpisode in newEpisodes)
                    {
                        db.Episodes.Add(newEpisode);
                    }
                }

                db.SaveChanges();
            }
        }

        public void DeleteFile(string filePath)
        {
            using(var db = new AlfredContext())
            {

                var oldFilm = (from f in db.Movies
                               where f.Path == filePath
                               select f).FirstOrDefault();

                if (oldFilm != null)
                    db.Movies.Remove(oldFilm);

                var oldMusique = (from f in db.Musics
                                  where f.Path == filePath
                                  select f).FirstOrDefault();

                if (oldMusique != null)
                    db.Musics.Remove(oldMusique);

                var oldSerie = (from f in db.Episodes
                                where f.Path == filePath
                                select f).FirstOrDefault();

                if (oldSerie != null)
                    db.Episodes.Remove(oldSerie);

                db.SaveChanges();
            }
        }

        public void RenameFile(string oldFilePath, string newFilePath)
        {
            using (var db = new AlfredContext())
            {
                var oldFilm = (from f in db.Movies
                               where f.Path == oldFilePath
                               select f).FirstOrDefault();

                if (oldFilm != null)
                    oldFilm.Path = newFilePath;

                var oldMusique = (from f in db.Musics
                                  where f.Path == oldFilePath
                                  select f).FirstOrDefault();

                if (oldMusique != null)
                    oldMusique.Path = newFilePath;

                var oldSerie = (from f in db.Episodes
                                where f.Path == oldFilePath
                                select f).FirstOrDefault();

                if (oldSerie != null)
                    oldSerie.Path = newFilePath;

                db.SaveChanges();
            }
        }
    }
}
