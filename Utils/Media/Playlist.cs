using System.Collections.Generic;
using Alfred.Model.Db;

namespace Alfred.Utils
{
    public class PlaylistChannel
    {
        public int Channel;
        public SortedList<int, Music> Musiques;

        public PlaylistChannel(int channel = 0)
        {
            Channel = channel;
            Musiques = new SortedList<int, Music>();
        }
    }
}
