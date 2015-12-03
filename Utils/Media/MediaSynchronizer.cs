using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Alfred.Model.Db;
using log4net;
using System;
using Alfred.Model.Db.Repositories;

namespace Alfred.Utils
{
    public class MediaSynchronizer
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MediaSynchronizer));

        public string[] AudioExt = { ".mp3", ".flac", ".m4a", ".wma", ".ogg" };
        public string[] MovieExt = { ".avi", ".m4a", ".mkv", ".mp4" };
        public IDictionary<string, string> paths;
        private MediaRepository _repo;

        public MediaSynchronizer()
        {
            paths = new PathRepository().GetPaths();
            _repo = new MediaRepository();
        }

        public void FillDBFromFiles()
        {
            try
            {
                _repo.SaveFromFiles(AudioExt, MovieExt);
            }
            catch(Exception e)
            {
                log.ErrorFormat("An error occured when synchronizing media : {0}", e);
            }
        }

        public void AddFile(string filePath)
        {
            var dirMusic = new DirectoryInfo(paths["Music"]);
            var dirMovies = new DirectoryInfo(paths["Movies"]);
            var dirSeries = new DirectoryInfo(paths["Series"]);
            var file = new FileInfo(filePath);

            if (_repo.MediaExists(filePath))
                return;

            var fileInfo = new List<string>();

            if (filePath.Contains(dirMusic.FullName))
            {
                var parentDir = file.Directory;
                while (parentDir != null && parentDir.FullName != dirMusic.FullName)
                {
                    fileInfo.Add(parentDir.Name);
                    parentDir = parentDir.Parent;
                }

                Music musique = null;
                if (fileInfo.Count == 3)
                    musique = new Music { Genre = fileInfo[2], Artist = fileInfo[1], Album = fileInfo[0], Title = file.Name, Path = file.FullName };
                else if (fileInfo.Count == 2)
                    musique = new Music { Genre = fileInfo[1], Artist = fileInfo[0], Album = null, Title = file.Name, Path = file.FullName };

                _repo.SaveMusic(musique);
            }
            else if (filePath.Contains(dirMovies.FullName))
            {
                var parentDir = file.Directory;
                while (parentDir != null && parentDir.FullName != dirMusic.FullName)
                {
                    fileInfo.Add(parentDir.Name);
                    parentDir = parentDir.Parent;
                }

                var film = new Movie { Genre = fileInfo[fileInfo.Count - 1], Title = file.Name, Path = file.FullName };
                _repo.SaveMovie(film);
            }
            else if (filePath.Contains(dirSeries.FullName))
            {
                var parentDir = file.Directory;
                while (parentDir != null && parentDir.FullName != dirMusic.FullName)
                {
                    fileInfo.Add(parentDir.Name);
                    parentDir = parentDir.Parent;
                }

                Episode episode = null;
                if (fileInfo.Count == 2)
                {
                    byte resultString;
                    var isParsed = byte.TryParse(Regex.Match(fileInfo[1], @"\d+").Value, out resultString);
                    episode = new Episode { ShowName = fileInfo[2], SeasonNumber = isParsed ? resultString : (byte)1, Path = file.FullName };
                }
                else if (fileInfo.Count == 1)
                    episode = new Episode { ShowName = fileInfo[1], Path = file.FullName };

                _repo.SaveEpisode(episode);
            }
        }

        public void DeleteFile(string filePath)
        {
            _repo.DeleteFile(filePath);
        }

        public void RenameFile(string oldFilePath, string newFilePath)
        {
            _repo.RenameFile(oldFilePath, newFilePath);
        }
    }
}
