using Alfred.Model.Db.Repositories;
using System.IO;
using System.Linq;

namespace Alfred.Utils.Server
{
    public class MediaWatcher
    {
        public string[] AudioExt = { ".mp3", ".flac", ".m4a", ".wma", ".ogg" };
        public string[] MovieExt = { ".avi", ".m4a", ".mkv" };

        public static string CurrentFilePath;
        public static string NewFilePath;

        private readonly MediaSynchronizer _mm;

        public MediaWatcher()
        {
            var paths = new PathRepository().GetPaths();
            _mm = new MediaSynchronizer();

            if (paths.ContainsKey("Music") && Directory.Exists(paths["Music"]))
            {
                var watcher1 = new FileSystemWatcher(paths["Music"]) { IncludeSubdirectories = true, EnableRaisingEvents = true };
                watcher1.Created += watcher_Created;
                watcher1.Deleted += watcher_Deleted;
                watcher1.Renamed += watcher_Renamed;
            }

            if (paths.ContainsKey("Movies") && Directory.Exists(paths["Movies"]))
            {
                var watcher2 = new FileSystemWatcher(paths["Movies"]) { IncludeSubdirectories = true, EnableRaisingEvents = true };
                watcher2.Created += watcher_Created;
                watcher2.Deleted += watcher_Deleted;
                watcher2.Renamed += watcher_Renamed;
            }

            if (paths.ContainsKey("Series") && Directory.Exists(paths["Series"]))
            {
                var watcher3 = new FileSystemWatcher(paths["Series"]) { IncludeSubdirectories = true, EnableRaisingEvents = true };
                watcher3.Created += watcher_Created;
                watcher3.Deleted += watcher_Deleted;
                watcher3.Renamed += watcher_Renamed;
            }
        }

        void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            _mm.RenameFile(e.OldFullPath, e.FullPath);
        }

        void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            _mm.DeleteFile(e.FullPath);
        }

        void watcher_Created(object sender, FileSystemEventArgs e)
        {
            var file = new FileInfo(e.FullPath);
            if(AudioExt.Contains(file.Extension) || MovieExt.Contains(file.Extension))
                _mm.AddFile(e.FullPath);
        }
    }
}
