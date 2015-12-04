namespace Alfred.Client.Gui.Controllers.Ink
{
    public class GroovesharkMusic
    {
        private string _songName;
        private string _artistName;

        public string Url;
        public int SongID;
        public string SongName { 
            get { return _songName; }
            set { _songName = value; }
        }
        public int ArtistID;
        public string ArtistName { 
            get { return _artistName; }
            set { _artistName = value; }
        }
        public int AlbumID;
        public string AlbumName;
    }
}
