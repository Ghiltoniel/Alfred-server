using System.IO;
using System.Linq;
using AlfredPlayer.Controllers.Players;

namespace AlfredPlayer
{
    class Players
    {
        public static GroovesharkPlayer GroovesharkPlayer;
        public static DigitalyImportedPlayer DigitalyImportedPlayer;
        public static UrlPlayer UrlPlayer;
        public static WMPPlayer WMPPlayer;
        public static VlcPlayer VlcPlayer;
        public static string[] movieExt = { ".avi", ".m4a", ".mkv", ".mp4" };
        
        public static APlayer GetSpecificPlayer(string inputFile)
        {
            var extension = inputFile.Substring(inputFile.Length - 4);
            if (inputFile.Contains("tinysong"))
            {
                if (GroovesharkPlayer == null)
                    GroovesharkPlayer = new GroovesharkPlayer();
                return GroovesharkPlayer;
            }
            if (inputFile.Contains("www.di.fm"))
            {
                if (DigitalyImportedPlayer == null)
                    DigitalyImportedPlayer = new DigitalyImportedPlayer();
                return DigitalyImportedPlayer;
            }
            if (inputFile.StartsWith("http://") || inputFile.StartsWith("https://"))
            {
                if (UrlPlayer == null)
                    UrlPlayer = new UrlPlayer();
                return UrlPlayer;
            }
            if (File.Exists(inputFile) && movieExt.Contains(extension))
            {
                if (VlcPlayer == null)
                    VlcPlayer = new VlcPlayer();
                return VlcPlayer;
            }
            if (File.Exists(inputFile))
            {
                if (WMPPlayer == null)
                    WMPPlayer = new WMPPlayer();
                return WMPPlayer;
            }
            return null;
        }
    }
}
