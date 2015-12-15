using System.Collections.Generic;
using System.Diagnostics;

namespace Alfred.Utils
{
    class Show
    {
        public string Name { get; set; }
        public string PathShow { get; set; }
        public string PathSeason { get; set; }
        public string PathEpisode { get; set; }
        public int Season { get; set; }
        public int Episode { get; set; }

        public static List<string> getShowsList()
        {
            var showList = new List<string>();
            return showList;
        }

        public bool hasSeason()
        {
            return false;
        }

        public bool hasEpisode()
        {
            return false;
        }

        public void Play()
        {
            Process.Start(PathEpisode);
        }
    }
}
